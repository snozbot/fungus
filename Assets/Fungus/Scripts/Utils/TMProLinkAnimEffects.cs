using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER

namespace Fungus
{
    /// <summary>
    /// Samples and helpers for creating TMProLink animations 
    /// </summary>
    namespace TMProLinkAnimEffects
    {
        public enum TMPLinkAnimatorMode
        {
            PerCharacter,
            PerWord,
            PerSection,
            PerLine,
        }

        /// <summary>
        /// Use of this is not required, all that the TMProLinkAnimLookup requires is a matching signature of
        /// void delegate(TMProLinkAnimator context, int start, int length). The base class however is used to
        /// create all the sample effects as they all operate in a similar underlying fashion, with a custom mode 
        /// but ultimately on a character by character basis, doing a relative translation and color modifcation.
        /// 
        /// Much of this class and its sample child effects are static functions to more easily allow reuse by 
        /// custom effects you may wish to make.
        /// </summary>
        public abstract class BaseEffect
        {
            public TMPLinkAnimatorMode mode;

            public virtual void DoEffect(TMProLinkAnimator context, int start, int length)
            {
                MeshVertUpdateLoop(context, start, length, TransFunc, ColorFunc, mode);
            }

            public virtual Matrix4x4 TransFunc(int index)
            {
                return Matrix4x4.identity;
            }

            public virtual Color32 ColorFunc(int index, Color32 col)
            {
                return col;
            }

            #region static helpers
            //Helper for mesh vertex updating within a found link
            //adapted from TMPRo examples VertexJitter
            static public void MeshVertUpdateLoop(TMProLinkAnimator context, int start, int length, System.Func<int, Matrix4x4> transformFunc, System.Func<int, Color32, Color32> colorFunc, TMPLinkAnimatorMode mode)
            {
                var tmproComponent = context.TMProComponent;
                var textInfo = tmproComponent.textInfo;
                var end = start + length;
                var cachedMeshInfo = context.CachedMeshInfo;

                Matrix4x4 matrix = Matrix4x4.identity;
                Vector2 middle = Vector2.zero;
                Color32 col = Color.white;
                int wordIndex = -1;
                int lineIndex = -1;

                for (int i = start; i < end; i++)
                {
                    //required as TMPro is putting non visible invalid elements in the charinfo array assuming I will follow this rule
                    //  if we don't character 0 ends up getting an effect applied to it even though it shouldn't
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Get the cached vertices of the mesh used by this text element (character or sprite).
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                    Color32[] vertexColors = cachedMeshInfo[materialIndex].colors32;

                    if (i == start && mode == TMPLinkAnimatorMode.PerSection)
                    {
                        matrix = transformFunc(start);
                        middle = CalcMidFromChars(context, start, end);
                        col = colorFunc(start, vertexColors[vertexIndex]);
                    }

                    // Determine the center point of each character at the baseline.
                    //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                    // Determine the center point of each character.
                    if (mode == TMPLinkAnimatorMode.PerCharacter)
                    {
                        middle = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                        matrix = transformFunc(i);
                        col = colorFunc(i, vertexColors[vertexIndex]);
                    }
                    else if (mode == TMPLinkAnimatorMode.PerWord)
                    {
                        var newWordIndex = CalcWordFromChar(i, textInfo.wordInfo);
                        if (newWordIndex != -1 && wordIndex != newWordIndex)
                        {
                            wordIndex = newWordIndex;

                            middle = CalcMidFromChars(context, Mathf.Max(start, textInfo.wordInfo[wordIndex].firstCharacterIndex), Mathf.Min(end, textInfo.wordInfo[wordIndex].lastCharacterIndex));
                            matrix = transformFunc(i);
                            col = colorFunc(i, vertexColors[vertexIndex]);
                        }
                    }
                    else if (mode == TMPLinkAnimatorMode.PerLine)
                    {
                        var newLineIndex = textInfo.characterInfo[i].lineNumber;
                        if (newLineIndex != -1 && lineIndex != newLineIndex)
                        {
                            lineIndex = newLineIndex;

                            middle = CalcMidFromChars(context, Mathf.Max(start, textInfo.lineInfo[lineIndex].firstCharacterIndex), Mathf.Min(end, textInfo.lineInfo[lineIndex].lastCharacterIndex));
                            matrix = transformFunc(i);
                            col = colorFunc(i, vertexColors[vertexIndex]);
                        }
                    }

                    // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                    // This is needed so the matrix TRS is applied at the origin for each character.
                    Vector3 offset = middle;

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                    Color32[] destinationVertColors = textInfo.meshInfo[materialIndex].colors32;

                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;


                    destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                    destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                    destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                    destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                    destinationVertices[vertexIndex + 0] += offset;
                    destinationVertices[vertexIndex + 1] += offset;
                    destinationVertices[vertexIndex + 2] += offset;
                    destinationVertices[vertexIndex + 3] += offset;

                    destinationVertColors[vertexIndex + 0] = col;
                    destinationVertColors[vertexIndex + 1] = col;
                    destinationVertColors[vertexIndex + 2] = col;
                    destinationVertColors[vertexIndex + 3] = col;
                }
            }

            //same as calcing a character mid but averaging over all characters in the given character range
            static public Vector2 CalcMidFromChars(TMProLinkAnimator context, int start, int end)
            {
                Vector3 middle = Vector3.zero;
                var chInfo = context.TMProComponent.textInfo.characterInfo;

                for (int i = start; i < end; i++)
                {
                    int materialIndex = chInfo[i].materialReferenceIndex;
                    int vertexIndex = chInfo[i].vertexIndex;
                    Vector3[] sourceVertices = context.CachedMeshInfo[materialIndex].vertices;
                    middle += (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                }

                return middle / (end - start);
            }

            static public int CalcWordFromChar(int charIndex, TMPro.TMP_WordInfo[] wordInfo)
            {
                for (int i = 0; i < wordInfo.Length; i++)
                {
                    //enforcing start letter results in punctuation attaching to the word to its left rather than its right.
                    //  which is more desirable for english at least
                    if (charIndex >= wordInfo[i].firstCharacterIndex && wordInfo[i].lastCharacterIndex >= charIndex)
                    {
                        return i;
                    }
                }

                return -1;
            }

            static public int CalcLineFromChar(int charIndex, TMPro.TMP_WordInfo[] wordInfo)
            {
                for (int i = 0; i < wordInfo.Length; i++)
                {
                    //enforcing start letter results in punctuation attaching to the word to its left rather than its right.
                    //  which is more desirable for english at least
                    if (charIndex >= wordInfo[i].firstCharacterIndex && wordInfo[i].lastCharacterIndex >= charIndex)
                    {
                        return i;
                    }
                }

                return -1;
            }

            static public void CharRangeToWordIndices(int startCharacter, int length, TMPro.TMP_WordInfo[] wordInfo, out int firstWord, out int lastWord)
            {
                firstWord = -1;
                lastWord = -1;

                int endCharacter = startCharacter + length;

                firstWord = CalcWordFromChar(startCharacter, wordInfo);
                lastWord = CalcWordFromChar(endCharacter, wordInfo);
            }
            #endregion
        }

        public class ShakeEffect : BaseEffect
        {
            public float offsetScale, rotScale;

            public override Matrix4x4 TransFunc(int index)
            {
                return ShakeTransformFunc(index, offsetScale, rotScale);
            }

            static public Matrix4x4 ShakeTransformFunc(int index, float positionOffsetScale, float rotDegScale)
            {
                return Matrix4x4.TRS(new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0) * positionOffsetScale,
                    Quaternion.Euler(0, 0, Random.Range(-1f, 1f) * rotDegScale),
                    Vector3.one);
            }

        }

        public class WiggleEffect : BaseEffect
        {
            public float scale;

            public override Matrix4x4 TransFunc(int index)
            {
                return WiggleTransformFunc(index, scale);
            }

            static public Matrix4x4 WiggleTransformFunc(int index, float wiggleScale)
            {
                const int SAFE_PRIME_A = 11;
                const int SAFE_PRIME_B = 59;
                //add a pingpong
                var jitterOffset = new Vector3(Mathf.PerlinNoise(Time.time + index * SAFE_PRIME_A, index * SAFE_PRIME_B),
                                               Mathf.PerlinNoise(Time.time + index * SAFE_PRIME_B, index * SAFE_PRIME_A),
                                               0);
                jitterOffset *= 2;
                jitterOffset -= new Vector3(1, 1, 0);

                return Matrix4x4.TRS(jitterOffset * wiggleScale,
                    Quaternion.identity,
                    Vector3.one);
            }
        }

        public class WaveEffect : BaseEffect
        {
            public float speed, indexStep, scale;

            public override Matrix4x4 TransFunc(int index)
            {
                return WaveTransformFunc(index, speed, indexStep, scale);
            }

            static public Matrix4x4 WaveTransformFunc(int index, float waveSpeed, float waveIndexStep, float waveScale)
            {
                return Matrix4x4.TRS(new Vector3(0, Mathf.Sin(Time.time * waveSpeed + index * waveIndexStep) * waveScale, 0),
                    Quaternion.identity,
                    Vector3.one);
            }
        }

        public class PivotEffect : BaseEffect
        {
            public float speed, degScale;

            public override Matrix4x4 TransFunc(int index)
            {
                return PivotTransformFunc(index, speed, degScale);
            }

            static public Matrix4x4 PivotTransformFunc(int index, float pivotSpeed, float pivotDegScale)
            {
                return Matrix4x4.TRS(Vector3.zero,
                      Quaternion.Euler(0, 0, Mathf.Sin(Time.time * pivotSpeed + index) * pivotDegScale),
                      Vector3.one);
            }
        }

        public class RainbowEffect : BaseEffect
        {
            public float speed, indexStep, s, v;

            public override Color32 ColorFunc(int index, Color32 col)
            {
                return CycleColor(index, speed, indexStep, s, v);
            }

            static public Color32 CycleColor(int index, float speed, float indexStep, float s, float v)
            {
                float h = Time.time * speed + index * indexStep;
                var col = Color.HSVToRGB(h % 1.0f, s, v);
                return (Color32)col;
            }
        }   
    }
}
#endif