using System;

namespace BunknotesApp
{
	public enum CreateBunkNoteResult
	{
		BunkNoteAlreadySent = 0 ,
		HasNotBeenSent = 1,
		SentSuccessfully = 2,
		Error = 3
		// 0: "Your BunkNote has already been sent."
		// 1: "Your Note has NOT been sent.
		// You need to purchase additional credits in
		// order to send."

		// 2: "Your Note has been sent."

		// 3: "An error has occurred,
		// please contact the system administrator."
	}
}

