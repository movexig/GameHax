﻿using System;
using System.Collections.Generic;
using System.Xml;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;
using System.IO;

using MG.ParticleHax.Actions;

namespace MG.ParticleHax.Controllers
{
	class DocumentController
	{
		public const string ProjectFileExtension = ".pe";

		private MainController controller;
		private Model model;
		private const string projectFilters = "ParticleHax Projects (*" + ProjectFileExtension + ")|*" + ProjectFileExtension + "|All files (*.*)|*.*";

		public event Action NewDocument = delegate { };
		public event Action OpenDocument = delegate { };
		public event Action BeforeSaveDocument = delegate { };
		public event Action CloseDocument = delegate { };
		
		public DocumentController(MainController controller, Model model)
		{
			this.controller = controller;
			this.model = model;
		}

		public void New()
		{
			if (!Close()) return;

			Log.Info("New project created.");
			controller.UpdateTitle = true;
			NewDocument.Invoke();
		}

		public void Open()
		{
			var result = controller.ShowOpenDialog("Open ParticleHax Project...", projectFilters, "");
			if (result.Accepted)
			{
				Open(result.SelectedPath);
			}
		}

		public bool Open(FilePath file)
		{
			if (!Directory.Exists(file.ParentDirectory))
			{
				Log.Error("Tried to open file with non-existent folder: " + file.ParentDirectory);
				return false;
			}

			if (!Close()) return false;
			
			model.Clear();

			try
			{
				var xml = ParticleDefinitionTable.Open(file);
				var version = ParticleDefinitionTable.GetVersion(xml);
				if (version > ParticleDefinitionTable.CurrentVersion)
				{
					var r = controller.ShowMessageOkCancel("<b>Editor out of date!</b>\n\nThe file is saved in a newer format. You should update the editor to the newest version.\n\nContinue loading?", MainWindow.MessageType.Warning);

					if (r == MainWindow.ResponseType.Cancel)
					{
						Close();
						return false;
					}

					Log.Warning("- Loaded data of newer version.");
				}
				else if (version < ParticleDefinitionTable.CurrentVersion)
				{
					Log.Warning("- Loading data of older version.");
				}

				model.DefinitionTable.Load(xml);

				// Convert path parameters
				ToAbsolutePath(file.ParentDirectory, model.DefinitionTable.Definitions);

				// Add missing parameters
				AddMissingParametersRecursive(model.DefinitionTable.Definitions);

				Environment.CurrentDirectory = file.ParentDirectory;
			}
			catch (Exception e)
			{
				controller.ShowMessage("<b>Error on load!</b>\n\nMessage: " + e.Message, MainWindow.MessageType.Error);
				Log.Error("- Error: " + e.Message);
				return false;
			}

			model.DocumentFile = file;
			controller.UpdateTree = true;
			controller.UpdateTitle = true;
			Log.Info("Opened project: " + file);

			OpenDocument.Invoke();
			return true;
		}

		private void AddMissingParametersRecursive(ParticleCollection definitions)
		{
			foreach (var defPair in definitions)
			{
				ParticleDeclaration declaration;
				if (model.DeclarationTable.Declarations.TryGetValue(defPair.Declaration, out declaration))
				{
					AddAction.AddMissingParameters(declaration.Parameters, defPair.Parameters, false);
				}
				else
				{
					Log.Warning("Could not find declation: " + declaration);
				}

				AddMissingParametersRecursive(defPair.Children);
			}
		}
		
		public bool Close()
		{
			if (model.Modified)
			{
				var response = controller.ShowMessageYesNoCancel("This project contains unsaved changes. Do you want to save before you continue?", MainWindow.MessageType.Question);

				switch (response)
				{
					case MainWindow.ResponseType.Cancel:
					{
						return false;
					}

					case MainWindow.ResponseType.Yes:
					{
						if (!Save()) return false;
					}
					break;
				}

				Log.Info("Closed project.");
			}

			model.Clear();
			controller.UpdateTree = true;
			controller.UpdateTitle = true;
			CloseDocument.Invoke();

			return true;
		}

		public bool Save()
		{
			if (model.DocumentFile.IsNullOrEmpty)
			{
				return SaveAs();
			}
			
			return Save(model.DocumentFile);
		}

		public bool SaveAs()
		{
			var result = controller.ShowSaveDialog("Save ParticleHax Project...", projectFilters, model.DocumentFile);
			if (result.Accepted)
			{
				var outputFile = result.SelectedPath;
				if (!outputFile.HasExtension(ProjectFileExtension) && result.FilterIndex == 0)
				{
					outputFile = outputFile.ChangeExtension(ProjectFileExtension);
				}

				return Save(outputFile);
			}
			return false;
		}

		private bool Save(FilePath outputFile)
		{
			BeforeSaveDocument.Invoke();

			bool success = false;
			ToRelativePath(outputFile.ParentDirectory, model.DefinitionTable.Definitions);

			try
			{
				DocumentSaver.Save(model, outputFile);
				model.DocumentFile = outputFile;
				model.Modified = false;
				controller.UpdateTitle = true;
				success = true;

				Log.Info("Saved to file " + outputFile);
				Environment.CurrentDirectory = outputFile.ParentDirectory;
			}
			catch (Exception e)
			{
				controller.ShowMessage("<b>Error on save!</b>\n\nMessage: " + e.Message, MainWindow.MessageType.Error);
				Log.Error("- Error: " + e.Message);
			}

			ToAbsolutePath(Environment.CurrentDirectory, model.DefinitionTable.Definitions);

			return success;
		}
				
		public void Undo()
		{
			model.UndoHandler.Undo();
		}

		public void Redo()
		{
			model.UndoHandler.Redo();
		}

		private void ToAbsolutePath(string directory, ParticleCollection collection)
		{
			foreach (var d in collection)
			{
				ToAbsolutePath(directory, d.Parameters);

				foreach (var c in d.Children)
				{
					ToAbsolutePath(directory, c.Parameters);
				}
			}
		}

		private void ToRelativePath(string directory, ParticleCollection collection)
		{
			foreach (var d in collection)
			{
				ToRelativePath(directory, d.Parameters);

				foreach (var c in d.Children)
				{
					ToRelativePath(directory, c.Parameters);
				}
			}
		}

		private void ToAbsolutePath(string directory, Dictionary<string, ParticleDefinition.Parameter> parameters)
		{
			foreach (var param in parameters)
			{
				var v = param.Value.Value;
				if (v.IsFilePath()) // Paths are saved in relative format, convert to absolute format internally
				{
					v.Set((FilePath)v.Get<FilePath>().ToAbsolute(directory));
				}

				ToAbsolutePath(directory, param.Value.Parameters);
			}
		}

		private void ToRelativePath(string directory, Dictionary<string, ParticleDefinition.Parameter> parameters)
		{
			foreach (var param in parameters)
			{
				var v = param.Value.Value;
				
				if (v.IsFilePath()) // Save relative paths
				{
					v.Set((FilePath)v.Get<FilePath>().ToRelative(directory).ToString().Replace('\\', '/')); // Don't want constant changes depending on platform
				}

				ToRelativePath(directory, param.Value.Parameters);
			}
		}
	}
}
