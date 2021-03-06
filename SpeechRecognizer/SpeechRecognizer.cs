﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System.Text;
using System.Collections.Generic;

namespace KKSpeech {
	
	public enum AuthorizationStatus {
		Authorized,
		Denied,
		NotDetermined,
		Restricted
	}

	public struct SpeechRecognitionOptions {
		public bool shouldCollectPartialResults;
	}

	public struct LanguageOption {
		public readonly string id;
		public readonly string displayName;

		public LanguageOption(string id, string displayName) {
			this.id = id;
			this.displayName = displayName;
		}
	}
 
	public class SpeechRecognizer : System.Object {

		public static bool ExistsOnDevice() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			return AndroidSpeechRecognizer.EngineExists();
			#endif
			return false; 
		}

		public static void RequestAccess() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.RequestAccess();
			#endif
		}

		public static bool IsRecording() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			return AndroidSpeechRecognizer.IsRecording();
			#endif
			return false;
		}

		public static AuthorizationStatus GetAuthorizationStatus() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			return (AuthorizationStatus)AndroidSpeechRecognizer.AuthorizationStatus();
			#endif
			return AuthorizationStatus.NotDetermined;
		}

		public static void StopIfRecording() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StopIfRecording();
			#endif
		}

		private static void StartRecording(SpeechRecognitionOptions options) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StartRecording(options);
			#endif
		}

		public static void StartRecording(bool shouldCollectPartialResults) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StartRecording(shouldCollectPartialResults);
			#endif
		}

		public static void GetSupportedLanguages() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.GetSupportedLanguages();
			#endif
		}

		public static void SetDetectionLanguage(string languageID) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.SetDetectionLanguage(languageID);
			#endif
		}


		private class iOSSpeechRecognizer {

			[DllImport ("__Internal")]
			internal static extern void _SetDetectionLanguage(string languageID);

			[DllImport ("__Internal")]
			internal static extern string _SupportedLanguages();

			[DllImport ("__Internal")]
			internal static extern void _RequestAccess();

			[DllImport ("__Internal")]
			internal static extern bool _IsRecording();

			[DllImport ("__Internal")]
			internal static extern bool _EngineExists();

			[DllImport ("__Internal")]
			internal static extern int _AuthorizationStatus();

			[DllImport ("__Internal")]
			internal static extern void _StopIfRecording();

			[DllImport ("__Internal")]
			internal static extern void _StartRecording(bool shouldCollectPartialResults);

			public static void SupportedLanguages() {
				string formattedLangs = _SupportedLanguages();
				var listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
				if (listener != null) {
					listener.SupportedLanguagesFetched(formattedLangs);
				}
			}
		}

		private class AndroidSpeechRecognizer {

			private static string DETECTION_LANGUAGE = null;

			internal static void GetSupportedLanguages() {
				GetAndroidBridge().CallStatic("GetSupportedLanguages");
			}

			internal static void SetDetectionLanguage(string languageID) {
				AndroidSpeechRecognizer.DETECTION_LANGUAGE = languageID;
			}

			internal static void RequestAccess() {
				GetAndroidBridge().CallStatic("RequestAccess");
			}
				
			internal static bool IsRecording() {
				return GetAndroidBridge().CallStatic<bool>("IsRecording");
			}

			internal static bool EngineExists() {
				return GetAndroidBridge().CallStatic<bool>("EngineExists");
			}

			internal static int AuthorizationStatus() {
				return GetAndroidBridge().CallStatic<int>("AuthorizationStatus");
			}

			internal static void StopIfRecording() {
				GetAndroidBridge().CallStatic("StopIfRecording");
			}

			internal static void StartRecording(bool shouldCollectPartialResults) {
				var options = new SpeechRecognitionOptions();
				options.shouldCollectPartialResults = shouldCollectPartialResults;
				StartRecording(options);
			}

			internal static void StartRecording(SpeechRecognitionOptions options) {
				GetAndroidBridge().CallStatic("StartRecording", CreateJavaRecognitionOptionsFrom(options));
			}

			private static AndroidJavaObject CreateJavaRecognitionOptionsFrom(SpeechRecognitionOptions options) {
				var javaOptions = new AndroidJavaObject("kokosoft.unity.speechrecognition.SpeechRecognitionOptions");
				javaOptions.Set<bool>("shouldCollectPartialResults", options.shouldCollectPartialResults);
				javaOptions.Set<string>("languageID", DETECTION_LANGUAGE);
				return javaOptions;
			}

			private static AndroidJavaObject GetAndroidBridge() {
				var bridge = new AndroidJavaClass("kokosoft.unity.speechrecognition.SpeechRecognizerBridge");
				return bridge;
			}
		}
	}

}



