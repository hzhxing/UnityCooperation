using System;
using UnityEditor;

namespace Editor.Tools
{
    public class ProgressBarUtil
    {
        public static bool enable = true;

        private static bool mIsShow = false;
        public static bool IsShow
        {
            get
            {
                return mIsShow;
            }
        }

        private static string mTitle = String.Empty;
        public static string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                UpdateProgress();
            }
        }

        private static string mContent = String.Empty;
        public static string Content
        {
            get
            {
                return mContent;
            }
            set
            {
                mContent = value;
                UpdateProgress();
            }
        }

        private static float mPercent = 0.0f;
        public static float Percent
        {
            get
            {
                return mPercent;
            }
            set
            {
                mPercent = value;
                UpdateProgress();
            }
        }

        public static void Show()
        {
            if (mIsShow)
            {
                 return;
            }
            mIsShow = true;
            UpdateProgress();
        }

        public static void Show(string title, string content, float percent)
        {
            mTitle = title;
            mContent = content;
            mPercent = percent;

            if (mIsShow)
            {
                return;
            }
            mIsShow = true;
            UpdateProgress();
        }

        public static void Close()
        {
            if (!mIsShow)
            {
                return;
            }
            mIsShow = false;

            EditorUtility.ClearProgressBar();
        }

        private static void UpdateProgress()
        {
            if (!enable || !IsShow)
            {
                return;
            }

            EditorUtility.DisplayProgressBar(Title,Content,Percent);
        }
    }
}