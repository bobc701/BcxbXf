using System;
using System.Diagnostics;

/* ----------------------------------------
 * This is a placeholder for Android speech
 * ----------------------------------------
 */

//-1906.01: Need this attribute...
[assembly: Xamarin.Forms.Dependency(typeof(BCX.BCXB.CTextToSpeach))]
namespace BCX.BCXB {

   /// <summary>
	/// My own implie=ntaion of speech...
   /// -1906.01: Implement the interface...
	/// </summary>
	public class CTextToSpeach : BcxbXf.Services.ITextToSpeach { 


		public void Speak(string s) {
         // ------------------------------------------------------
         Debug.WriteLine("In CTextToSpeach.Speak");
		}


	}
}

