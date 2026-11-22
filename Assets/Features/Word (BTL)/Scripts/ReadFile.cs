
using System.IO;
using UnityEngine;
using TMPro;

public class ReadData
{
    private string content_file = "";
    private int index = 0;

    public string ReadFiles(string path)
    {
        string full_path = Application.dataPath + path;

        content_file = File.ReadAllText(full_path)
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");

        index = 0;

        return content_file;
    }

    public string ReadLine(int line)
    {
        string[] lines = content_file.Split('\n');

        return line >= 0 && line < lines.Length
            ? lines[line]
            : null;
    }

    public string getFiles()
    {
        return content_file;
    }

    public bool HasNextPage()
    {
        return index < content_file.Length;
    }

    public string GetNextPage(TMP_Text layout)
    {
        if (!HasNextPage())
            return "";

        string remainingText = content_file.Substring(index);

        layout.text = remainingText;
        layout.ForceMeshUpdate();

        TMP_TextInfo textInfo = layout.textInfo;
        Rect rect = layout.rectTransform.rect;

        int lastFitCharacter = -1;

        for (int i = 0; i < textInfo.lineCount; i++)
        {
            TMP_LineInfo line = textInfo.lineInfo[i];

            bool fits =
                line.ascender <= rect.yMax &&
                line.descender >= rect.yMin;

            if (!fits)
                break;

            lastFitCharacter = line.lastCharacterIndex;
        }

        if (lastFitCharacter < 0)
        {
            int nextWhitespace = 0;

            while (nextWhitespace < remainingText.Length &&
                   !char.IsWhiteSpace(remainingText[nextWhitespace]))
            {
                nextWhitespace++;
            }

            if (nextWhitespace < remainingText.Length)
                nextWhitespace++;

            if (nextWhitespace == 0)
                nextWhitespace = 1;

            string fallbackPage =
                remainingText.Substring(0, nextWhitespace);

            index += fallbackPage.Length;

            return fallbackPage;
        }

        TMP_CharacterInfo lastChar =
            textInfo.characterInfo[lastFitCharacter];

        int endIndex = lastChar.index + lastChar.stringLength;

        endIndex = Mathf.Clamp(
            endIndex,
            0,
            remainingText.Length
        );

        if (endIndex < remainingText.Length &&
            !char.IsWhiteSpace(remainingText[endIndex]))
        {
            int lastWhitespace = -1;

            for (int i = endIndex - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(remainingText[i]))
                {
                    lastWhitespace = i;
                    break;
                }
            }

            if (lastWhitespace >= 0)
            {
                endIndex = lastWhitespace + 1;
            }
            else
            {
                int nextWhitespace = endIndex;

                while (nextWhitespace < remainingText.Length &&
                       !char.IsWhiteSpace(remainingText[nextWhitespace]))
                {
                    nextWhitespace++;
                }

                endIndex = nextWhitespace;
            }
        }

        endIndex = Mathf.Clamp(
            endIndex,
            1,
            remainingText.Length
        );

        string page = remainingText.Substring(0, endIndex);

        index += endIndex;

        return page;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetIndex(int savedIndex)
    {
        index = Mathf.Clamp(
            savedIndex,
            0,
            content_file.Length
        );
    }
}
