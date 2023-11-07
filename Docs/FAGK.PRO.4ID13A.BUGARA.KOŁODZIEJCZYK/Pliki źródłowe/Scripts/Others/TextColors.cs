using UnityEngine;
using System.Collections;
using TMPro;

namespace TMPro.Examples
{
    /// <summary>
    /// Klasa odpowiedzialna za animację kolorów tekstu.
    /// </summary>
    public class TextColors : MonoBehaviour
    {
        private TMP_Text m_TextComponent;

        private Color32 blackColor = new(0, 0, 0, 255);
        private Color32 whiteColor = new(255, 255, 255, 255);
        private Color32 redColor = new(255, 0, 0, 255);

        private int colorIndex = 0;

        void Awake()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }

        void Start()
        {
            StartCoroutine(AnimateVertexColors());
        }

        /// <summary>
        /// Animuje kolory wierzchołków tekstu w nieskończoność.
        /// </summary>
        /// <returns>Coroutine</returns>
        IEnumerator AnimateVertexColors()
        {
            m_TextComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = m_TextComponent.textInfo;
            int currentCharacter = 0;
            Color32[] newVertexColors;

            while (true)
            {
                int characterCount = textInfo.characterCount;

                if (characterCount == 0)
                {
                    yield return new WaitForSeconds(0.25f);
                    continue;
                }

                int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

                if (textInfo.characterInfo[currentCharacter].isVisible)
                {
                    switch (colorIndex)
                    {
                        case 0:
                            newVertexColors[vertexIndex + 0] = blackColor;
                            newVertexColors[vertexIndex + 1] = blackColor;
                            newVertexColors[vertexIndex + 2] = blackColor;
                            newVertexColors[vertexIndex + 3] = blackColor;
                            break;
                        case 1:
                            newVertexColors[vertexIndex + 0] = whiteColor;
                            newVertexColors[vertexIndex + 1] = whiteColor;
                            newVertexColors[vertexIndex + 2] = whiteColor;
                            newVertexColors[vertexIndex + 3] = whiteColor;
                            break;
                        case 2:
                            newVertexColors[vertexIndex + 0] = redColor;
                            newVertexColors[vertexIndex + 1] = redColor;
                            newVertexColors[vertexIndex + 2] = redColor;
                            newVertexColors[vertexIndex + 3] = redColor;
                            break;
                    }

                    m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                }

                currentCharacter = (currentCharacter + 1) % characterCount;
                colorIndex = (colorIndex + 1) % 3;

                yield return new WaitForSeconds(0.1f); // Zmiana koloru co 0.1 sekundy.
            }
        }
    }
}
