﻿using System;
using System.ComponentModel;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.ParticleEditor.Actions;

namespace MG.ParticleEditor.Proxies
{
	abstract class BaseParameterProxy : ICustomTypeDescriptor
	{
		public static int UndoGroup = 965168485;

		protected class Changeset : UndoableParticleAction
		{
			private Model model;

			public ParticleDefinition CurrentDefinition;
			public ParticleDefinition ChangedDefinition;
			public ParticleDefinition OriginalDefinition;

			public Changeset(Model model, ParticleDefinition particleDefinition)
			{
				this.model = model;
				CurrentDefinitionId = particleDefinition.Id;
				CurrentDefinition = particleDefinition;
				ChangedDefinition = new ParticleDefinition(particleDefinition);
				OriginalDefinition = new ParticleDefinition(particleDefinition);
			}

			public Changeset(Model model, Changeset changeset)
			{
				this.model = model;
				CurrentDefinitionId = changeset.CurrentDefinitionId;
				CurrentParameter = changeset.CurrentParameter;
				CurrentDefinition = changeset.CurrentDefinition;
				ChangedDefinition = new ParticleDefinition(changeset.ChangedDefinition);
				OriginalDefinition = new ParticleDefinition(changeset.OriginalDefinition);
			}
			
			protected override bool CallExecute()
			{
				if (ChangedDefinition.Equals(OriginalDefinition))
				{
					return false;
				}

				model.Modified = true;
				CurrentDefinition.CopyFrom(ChangedDefinition);
				return true;
			}

			protected override void CallUndo()
			{
				CurrentDefinition.CopyFrom(OriginalDefinition);
			}

			public override int GetUndoGroup()
			{
				return UndoGroup;
			}
		}
		
		protected Model model;
		protected ParticleDeclaration particleDeclaration;
		protected ParticleDefinition particleDefinition;
		protected Changeset changeset;
		
		public UndoableAction CommitAction()
		{
			// Move all changes (which are done in the CURRENT definition) to the CHANGED definition.
			changeset.ChangedDefinition.CopyFrom(changeset.CurrentDefinition);

			// Copy the changeset that is about to be committed.
			var copy = new Changeset(model, changeset);

			// Rebase the ORIGINAL definition so that we save changes from now -> next commit.
			changeset.OriginalDefinition.CopyFrom(changeset.CurrentDefinition);

			return copy;
		}

		public BaseParameterProxy(Model model, ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
		{
			this.model = model;
			this.particleDeclaration = particleDeclaration;
			this.particleDefinition = particleDefinition;
			changeset = new Changeset(model, particleDefinition);
		}
		
		//--------------------------------------
		// ICustomTypeDescriptor
		//--------------------------------------
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return GetProperties(null); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }

		public abstract PropertyDescriptorCollection GetProperties(Attribute[] attributes);
	}
}