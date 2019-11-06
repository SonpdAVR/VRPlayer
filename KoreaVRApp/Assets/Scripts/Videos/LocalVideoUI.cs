﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using CielaSpike;
using EasyMobile;
using System.IO;

public class LocalVideoUI: VideoUI
{
	[SerializeField] protected RawImage videoImage;
	[SerializeField] protected Text videoTitle;
	[SerializeField] protected Text videoLength_videoSize;

	// Use this for initialization
	void Start () 
	{
        thumbnailTexture = new Texture2D(4, 4, TextureFormat.RGB565, false);
    }

	protected bool loadThumbnail;

	public override void Update ()
	{
		if (loadThumbnail) {
			StartCoroutine(LoadThumbnail ());
			loadThumbnail = false;
		}
		if(pendingDelete)
		{
			StartCoroutine(DeleteProcess());
			pendingDelete = false;
		}
	}

	public override void Setup(Video currentlocalVideo)
	{
		base.Setup (currentlocalVideo);

		videoTitle.text = Path.GetFileNameWithoutExtension ((video as LocalVideo).videoURL); // Exeption "type .mp4"

		videoLength_videoSize.text = MakeLengthString () + " / " + ((video as LocalVideo).videoSize / 1024f / 1024f).ToString("0.0") + " MB";
        //videoImage.texture = null;

        SetPlayVideoBntLanguage();

    #if !UNITY_EDITOR
        loadThumbnail = true;
    #endif

    }
	
    IEnumerator LoadThumbnail()
    {
        bool gotThumbnail = false;

		string path = Application.persistentDataPath + "/localTemp/" + (video as LocalVideo).videoName;

#if UNITY_ANDROID
		path = Application.persistentDataPath + "/" + (video as LocalVideo).videoName;
#endif

#if UNITY_IOS
		path = Application.persistentDataPath + "/localTemp/" + (video as LocalVideo).videoName;
#endif

		//Texture2D texture = LocalVideoManager.instance.GetThumbnailFromCache (path);
		

	//	if (texture == null) {
			Debug.Log ("Looking at path: " + path);
			while (!gotThumbnail) {

				if (File.Exists (path)) {
					Debug.Log ("Found thumbnail at" + path);
                    LoadThumbnail (path);
					gotThumbnail = true;

					yield break;
				}

				yield return new WaitForSeconds (.5f);
			}
//		} else {
//			Debug.Log ("Texture found in cache, loading.....");
//		}

       
    }
		
#region NativeUI AlertPopup	
	/// <summary>
	/// Gets the alert when not loggin.
	/// </summary>
	public override void GetAlertDelete ()
	{
		base.GetAlertDelete ();
	}

	public override void OnAlertDeleteComplete ()
	{
        base.OnAlertDeleteComplete();
	}

#endregion

    public override void OnLoadedThumbnail()
    {
        videoImage.texture = thumbnailTexture;
        Debug.Log("------------------DONE 2D");
    }

	public override void RefreshCellView()
	{
		Setup(StorageMenu.instance.getVideoAtIndex(dataIndex));
	}

    #region Play 3D

    public override void PlayIn3D()
    {
        if (MainAllController.instance != null)
        {
            MainAllController.instance.PlayButtonSound();

            MainAllController.instance.GoVRPplayerMenu();

            MainAllController.instance.IsPlayVideo3D = true;

            MainAllController.instance.SetPlayVideo3DInfo(video, this);
        }
    }

    #endregion
}
