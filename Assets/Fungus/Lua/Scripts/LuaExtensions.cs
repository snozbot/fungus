/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Fungus;
using MoonSharp.Interpreter;

namespace Fungus
{

    public static class LuaExtensions 
    {

        /// <summary>
        /// Extension for MenuDialog that allows AddOption to call a Lua function when an option is selected.
        /// </summary>
        public static bool AddOption(this MenuDialog menuDialog, string text, bool interactable, LuaEnvironment luaEnvironment, Closure callBack)
        {
            if (!menuDialog.gameObject.activeSelf)
            {
                menuDialog.gameObject.SetActive(true);
            }

            bool addedOption = false;
            foreach (Button button in menuDialog.cachedButtons)
            {
                if (!button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);

                    button.interactable = interactable;

                    Text textComponent = button.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = text;
                    }

                    button.onClick.AddListener(delegate {

                        menuDialog.StopAllCoroutines(); // Stop timeout
                        menuDialog.Clear();
                        menuDialog.HideSayDialog();

                        if (callBack != null)
                        {
                            luaEnvironment.RunLuaFunction(callBack, true);
                        }
                    });

                    addedOption = true;
                    break;
                }
            }

            return addedOption;
        }

        /// <summary>
        /// Extension for MenuDialog that allows ShowTimer to call a Lua function when the timer expires.
        /// </summary>
        public static IEnumerator ShowTimer(this MenuDialog menuDialog, float duration, LuaEnvironment luaEnvironment, Closure callBack)
        {
            if (menuDialog.cachedSlider == null ||
                duration <= 0f)
            {
                yield break;
            }

            menuDialog.cachedSlider.gameObject.SetActive(true);
            menuDialog.StopAllCoroutines();

            float elapsedTime = 0;
            Slider timeoutSlider = menuDialog.GetComponentInChildren<Slider>();

            while (elapsedTime < duration)
            {
                if (timeoutSlider != null)
                {
                    float t = 1f - elapsedTime / duration;
                    timeoutSlider.value = t;
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            menuDialog.Clear();
            menuDialog.gameObject.SetActive(false);
            menuDialog.HideSayDialog();

            if (callBack != null)
            {
                luaEnvironment.RunLuaFunction(callBack, true);
            }
        }
    }

}