#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2018 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
#endregion

namespace Microsoft.Xna.Framework.Audio
{
	// http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.audio.cue.aspx
	public sealed class Cue : IDisposable
	{
		#region Public Properties

		public bool IsCreated
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_CREATED) != 0;
			}
		}

		public bool IsDisposed
		{
			get;
			private set;
		}

		public bool IsPaused
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_PAUSED) != 0;
			}
		}

		public bool IsPlaying
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_PLAYING) != 0;
			}
		}

		public bool IsPrepared
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_PREPARED) != 0;
			}
		}

		public bool IsPreparing
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_PREPARING) != 0;
			}
		}

		public bool IsStopped
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_STOPPED) != 0;
			}
		}

		public bool IsStopping
		{
			get
			{
				uint state;
				FAudio.FACTCue_GetState(handle, out state);
				return (state & FAudio.FACT_STATE_STOPPING) != 0;
			}
		}

		public string Name
		{
			get;
			private set;
		}

		#endregion

		#region Private Variables

		private IntPtr handle;
		private SoundBank bank;

		#endregion

		#region Disposing Event

		public event EventHandler<EventArgs> Disposing;

		#endregion

		#region Internal Constructor

		internal Cue(IntPtr cue, string name, SoundBank soundBank)
		{
			handle = cue;
			Name = name;
			bank = soundBank;
		}

		#endregion

		#region Destructor

		~Cue()
		{
			Dispose();
		}

		#endregion

		#region Public Dispose Method

		public void Dispose()
		{
			if (!IsDisposed)
			{
				if (Disposing != null)
				{
					Disposing.Invoke(this, null);
				}

				if (!bank.IsDisposed && !bank.engine.IsDisposed) // Just FYI, this is really bad
				{
					FAudio.FACTCue_Destroy(handle);
				}
				bank = null;

				IsDisposed = true;
			}
		}

		#endregion

		#region Public Methods

		public void Apply3D(AudioListener listener, AudioEmitter emitter)
		{
			if (listener == null)
			{
				throw new ArgumentNullException("listener");
			}
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}

			FAudio.F3DAUDIO_DSP_SETTINGS settings;
			settings.SrcChannelCount = 1;
			settings.DstChannelCount = bank.engine.channels;
			FAudio.FACT3DCalculate(
				bank.engine.handle3D,
				ref listener.listenerData,
				ref emitter.emitterData,
				out settings
			);
			FAudio.FACT3DApply(ref settings, handle);
		}

		public float GetVariable(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			ushort variable = FAudio.FACTCue_GetVariableIndex(
				handle,
				name
			);

			if (variable == FAudio.FACTVARIABLEINDEX_INVALID)
			{
				throw new InvalidOperationException(
					"Invalid variable name!"
				);
			}

			float result;
			FAudio.FACTCue_GetVariable(
				handle,
				variable,
				out result
			);
			return result;
		}

		public void Pause()
		{
			FAudio.FACTCue_Pause(handle, 1);
		}

		public void Play()
		{
			FAudio.FACTCue_Play(handle);
		}

		public void Resume()
		{
			FAudio.FACTCue_Pause(handle, 0);
		}

		public void SetVariable(string name, float value)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			ushort variable = FAudio.FACTCue_GetVariableIndex(
				handle,
				name
			);

			if (variable == FAudio.FACTVARIABLEINDEX_INVALID)
			{
				throw new InvalidOperationException(
					"Invalid variable name!"
				);
			}

			FAudio.FACTCue_SetVariable(
				handle,
				variable,
				value
			);
		}

		public void Stop(AudioStopOptions options)
		{
			FAudio.FACTCue_Stop(
				handle,
				(options == AudioStopOptions.Immediate) ?
					FAudio.FACT_FLAG_STOP_IMMEDIATE :
					FAudio.FACT_FLAG_STOP_RELEASE
			);
		}

		#endregion
	}
}
