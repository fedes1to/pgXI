using UnityEngine;

public class ReviewController : MonoBehaviour
{
	public const string keyNeedActiveReview = "keyNeedActiveReview";

	public const string keyAlreadySendReview = "keyAlreadySendReview";

	public const string keyOldVersionForReview = "keyOldVersionForReview";

	public const string keyNeedSendMsgReview = "keyNeedSendMsgReview";

	public const string keyReviewSaveRating = "keyReviewSaveRating";

	public const string keyReviewSaveMsg = "keyReviewSaveMsg";

	public static ReviewController instance;

	public static int _IsNeedActive = -1;

	public static int _ExistReviewForSend = -1;

	public static bool isSending;

	public static bool ExistReviewForSend
	{
		get
		{
			if (_ExistReviewForSend < 0)
			{
				_ExistReviewForSend = (Load.LoadBool("keyNeedSendMsgReview") ? 1 : 0);
			}
			return (_ExistReviewForSend != 0) ? true : false;
		}
		set
		{
			Save.SaveBool("keyNeedSendMsgReview", value);
			_ExistReviewForSend = (value ? 1 : 0);
		}
	}

	public static int ReviewRating
	{
		get
		{
			return Load.LoadInt("keyReviewSaveRating");
		}
		set
		{
			Save.SaveInt("keyReviewSaveRating", value);
		}
	}

	public static string ReviewMsg
	{
		get
		{
			return Load.LoadString("keyReviewSaveMsg");
		}
		set
		{
			Save.SaveString("keyReviewSaveMsg", value);
		}
	}

	public static bool IsSendReview
	{
		get
		{
			return Load.LoadBool("keyAlreadySendReview");
		}
		set
		{
			Save.SaveBool("keyAlreadySendReview", value);
		}
	}

	public static bool IsNeedActive
	{
		get
		{
			if (_IsNeedActive < 0)
			{
				_IsNeedActive = (Load.LoadBool("keyNeedActiveReview") ? 1 : 0);
			}
			return (_IsNeedActive != 0) ? true : false;
		}
		set
		{
			if (!ExistReviewForSend || !value)
			{
				if (IsNeedActive != value)
				{
					Save.SaveBool("keyNeedActiveReview", value);
				}
				_IsNeedActive = (value ? 1 : 0);
			}
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static void CheckActiveReview()
	{
		if (!(ExperienceController.sharedController == null) && GlobalGameController.CountDaySessionInCurrentVersion > 2 && ExperienceController.sharedController.currentLevel >= 4 && !IsSendReview)
		{
			IsNeedActive = true;
		}
	}

	public static void SendReview(int rating, string msgReview)
	{
		ReviewRating = rating;
		ReviewMsg = msgReview;
		ExistReviewForSend = true;
		if (rating == 5)
		{
			Application.OpenURL(Defs2.ApplicationUrl);
		}
		FriendsController.StartSendReview();
	}
}
