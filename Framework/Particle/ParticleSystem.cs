﻿using System.Collections.Generic;

using MG.Framework.Assets;
using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Utility;
using System;

namespace MG.Framework.Particle
{
	public class ParticleSystem
	{
		public Vector2 Position;
		public float Angle;
		
		public int ActiveParticles { get { return particleData.ActiveParticles; } }
		public readonly ParticleDefinition Definition;
		public List<ParticleSystem> SubSystems = new List<ParticleSystem>();

		private AssetHandler assetHandler;
		private ParticleSystemPool particleSystemPool;
		private Texture2D particleTexture;
		private List<Vector2> particlePosition;
		private List<Vector2> particleVelocity;
		private List<float> particleRotation;
		private List<float> particleLife;
		private List<float> particleAge;
		
		private ParticleData particleData = new ParticleData(64);
		private ParticleEmitter emitter;
		private bool disabled;
		
		private ParticleDefinition.Parameter paramTexture;
		private BlendMode paramBlendMode;
		private bool paramParticleInfinite;
		private Gradient paramParticleColor;
		private RandomFloat paramParticleScale;
		private RandomFloat paramParticleScaleX;
		private RandomFloat paramParticleScaleY;



		public ParticleSystem(AssetHandler assetHandler, ParticleSystemPool particleSystemPool, ParticleDefinition particleDefinition)
		{
			if (assetHandler == null) throw new ArgumentException("assetHandler");
			if (particleSystemPool == null) throw new ArgumentException("particleSystemPool");
			if (particleDefinition == null) throw new ArgumentException("particleDefinition");

			this.assetHandler = assetHandler;
			this.particleSystemPool = particleSystemPool;
			this.Definition = particleDefinition;
			
			particlePosition = particleData.Register<Vector2>("Position");
			particleVelocity = particleData.Register<Vector2>("Velocity");
			particleRotation = particleData.Register<float>("Rotation");
			particleLife = particleData.Register<float>("Life");
			particleAge = particleData.Register<float>("Age");
			
			emitter = new PointEmitter(particleData, particleDefinition);
		}
		
		public void Reload()
		{
			paramTexture = Definition.Parameters["Texture"];
			paramBlendMode = (BlendMode)Definition.Parameters["BlendMode"].Value.Get<int>();
			paramParticleInfinite = Definition.Parameters["ParticleInfinite"].Value.Get<bool>();
			paramParticleColor = Definition.Parameters["ParticleColor"].Value.Get<Gradient>();
			paramParticleScale = Definition.GetFloatParameter("ParticleScale");
			paramParticleScaleX = Definition.GetFloatParameter("ParticleScaleX");
			paramParticleScaleY = Definition.GetFloatParameter("ParticleScaleY");
			
			var texture = paramTexture.Value.Get<FilePath>();
			particleTexture = assetHandler.Load<Texture2D>(texture);
			emitter.Reload();

			if (Definition.Children.Count != SubSystems.Count)
			{
				ClearChildren();
				SubSystems.Capacity = Definition.Children.Count;
				foreach (var child in Definition.Children)
				{
					SubSystems.Add(particleSystemPool.Create(child));
				}
			}
		}

		public void Clear()
		{
			Position = Vector2.Zero;
			particleData.ActiveParticles = 0;
			emitter.Clear();
			disabled = false;

			ClearChildren();
		}

		private void ClearChildren()
		{
			foreach (var child in SubSystems)
			{
				child.Clear();
				particleSystemPool.Destroy(child);
			}
			SubSystems.Clear();
		}

		public bool Disabled
		{
			get { return disabled; }
			set
			{
				disabled = value;
				foreach (var system in SubSystems)
				{
					system.Disabled = true;
				}
			}
		}

		public bool Dead
		{
			get
			{
				if (!Disabled)
				{
					if (emitter.Alive) return false;
					if (paramParticleInfinite) return false;
				}
				
				if (particleData.ActiveParticles > 0) return false;

				foreach (var system in SubSystems)
				{
					if (!system.Dead) return false;
				}

				return true;
			}
		}

		public void Update(Time time)
		{
			if (time.ElapsedSeconds <= 0)
				return;

			((PointEmitter)emitter).Point = Position;
			if (!Disabled)
			{
				emitter.Update(time);
			}
			
			for (int i = 0; i < particleData.ActiveParticles;)
			{
				particlePosition[i] += particleVelocity[i] * time.ElapsedSeconds;
				particleAge[i] += time.ElapsedSeconds;

				if (!paramParticleInfinite && particleAge[i] >= particleLife[i])
				{
					emitter.Destroy(i);
				}
				else
				{
					i++;
				}
			}

			foreach (var system in SubSystems)
			{
				system.Update(time);
			}
		}

		public void Draw(RenderContext renderContext)
		{
			Draw(renderContext, Matrix.Identity);
		}

		public void Draw(RenderContext renderContext, Matrix transform)
		{
			var quadBatch = renderContext.QuadBatch;
			
			// TODO: Figure out the best blending mode
			if (paramBlendMode == BlendMode.BlendmodeAlpha)
			{
				paramBlendMode = BlendMode.BlendmodeNonPremultiplied;
			}

			quadBatch.Begin(transform, paramBlendMode);
			
			for (int i = 0; i < particleData.ActiveParticles; i++)
			{
				var p = particlePosition[i];
				var a = particleAge[i];
				var l = particleLife[i];
				var r = particleRotation[i];
				var lifeFraction = a / l;
				var color = paramParticleColor.Evaluate(lifeFraction);
				var s = paramParticleScale.Get(emitter.LifeFractional, lifeFraction);
				var sx = paramParticleScaleX.Get(emitter.LifeFractional, lifeFraction);
				var sy = paramParticleScaleY.Get(emitter.LifeFractional, lifeFraction);
				
				quadBatch.Draw(particleTexture, MathTools.Create2DAffineMatrix(p.X, p.Y, s * sx, s * sy, r), color, particleTexture.Size / 2, 0);
			}
			
			quadBatch.End();

			var childTransform = transform * MathTools.Create2DAffineMatrix(Position.X, Position.Y, 1, 1, 0);
			foreach (var system in SubSystems)
			{
				system.Draw(renderContext, childTransform);
			}
		}
	}
}
