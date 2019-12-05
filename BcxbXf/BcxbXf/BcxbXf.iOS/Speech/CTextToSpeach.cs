using System;
using AVFoundation;
using System.Diagnostics;

//-1906.01: Need this attribute...
[assembly: Xamarin.Forms.Dependency(typeof(BCX.BCXB.CTextToSpeach))]
namespace BCX.BCXB {

   /// <summary>
	/// My own implie=ntaion of speech...
   /// -1906.01: Implement the interface...
	/// </summary>
	public class CTextToSpeach : BcxbXf.Services.ITextToSpeach { 

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

      private AVSpeechSynthesisVoice GetVoiceByName(string name) {
         // -----------------------------------------------------
         foreach (AVSpeechSynthesisVoice v in AVSpeechSynthesisVoice.GetSpeechVoices()) {
            if (v.Name.Contains(name)) return v; //E.g., There is 'Samantha (Enhanced)'
         }
         return null;

      }



      private void GetVoices() {
      // -----------------------------------------------------
			foreach (AVSpeechSynthesisVoice v in AVSpeechSynthesisVoice.GetSpeechVoices()) {
				Debug.WriteLine(v.Name + ", " + v.Language + ", " + v.Identifier);
			}

		} 

	}
}

