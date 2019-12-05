using System;
using AVFoundation;
using System.Diagnostics;

namespace BCX.BCXB {

   /// <summary>
	/// My own implie=ntaion of speech...
	/// </summary>
	public class CTextToSpeach { 

		public AVSpeechSynthesizer speech;

		public CTextToSpeach () {
      // ------------------------------------------------------
			speech = new AVSpeechSynthesizer ();
         GetVoices ();
		}


		public void Speak(string s) {
      // ------------------------------------------------------
			var utter = new AVSpeechUtterance (s);
         //utter.Rate = AVSpeechUtterance.MaximumSpeechRate / 2;
         utter.Rate = AVSpeechUtterance.DefaultSpeechRate;

         // Aaron's Id = 'siri_male_en-US_compact'...    
         AVSpeechSynthesisVoice v = GetVoiceByName("Aaron"); 
         if (v != null) utter.Voice = v;
         else utter.Voice = AVSpeechSynthesisVoice.FromLanguage("en-US");
         Debug.Print("Voice used=" + utter.Voice?.Name ?? "None");

         utter.PitchMultiplier = 0.65F;
			speech.SpeakUtterance (utter);
		   
		}

      public AVSpeechSynthesisVoice GetVoiceByName(string name) {
         // -----------------------------------------------------
         foreach (AVSpeechSynthesisVoice v in AVSpeechSynthesisVoice.GetSpeechVoices()) {
            if (v.Name.Contains(name)) return v; //E.g., There is 'Samantha (Enhanced)'
         }
         return null;

      }



      public void GetVoices() {
      // -----------------------------------------------------
			foreach (AVSpeechSynthesisVoice v in AVSpeechSynthesisVoice.GetSpeechVoices()) {
				Debug.WriteLine(v.Name + ", " + v.Language + ", " + v.Identifier);
			}

		} 

	}
}

