using System.Collections.Generic;

namespace Liquid.BriskUI
{
    public class TextContent
    {
        private readonly string textID;
        private readonly string[] strParamArr;
        private readonly TextContent[] tcParmaArr;
        private readonly Dictionary<string, string> strParamDic;
        private readonly Dictionary<string, TextContent> tcParamDic;

        public static implicit operator string(TextContent t)
        {
            if (t == null)
                return null;

            return t.GetText();
        }

        public TextContent(string tid)
        {
            textID = tid;
            strParamArr = null;
            tcParmaArr = null;
            strParamDic = null;
            tcParamDic = null;
        }

        public TextContent(string tid, params string[] textParam)
        {
            textID = tid;
            strParamArr = textParam;
            tcParmaArr = null;
            strParamDic = null;
            tcParamDic = null;
        }

        public TextContent(string tid, params TextContent[] textParam)
        {
            textID = tid;
            strParamArr = null;
            tcParmaArr = textParam;
            strParamDic = null;
            tcParamDic = null;
        }

        public TextContent(string tid, Dictionary<string, string> textParam)
        {
            textID = tid;
            strParamArr = null;
            tcParmaArr = null;
            strParamDic = textParam;
            tcParamDic = null;
        }

        public TextContent(string tid, Dictionary<string, TextContent> textParam)
        {
            textID = tid;
            strParamArr = null;
            tcParmaArr = null;
            strParamDic = null;
            tcParamDic = textParam;
        }

        private string GetText()
        {
            if(textID == null)
                return "";

            if (strParamArr == null && strParamDic == null)
                return TextManager.Instance.GetText(textID);
            else if (strParamArr != null && strParamDic == null)
                return TextManager.Instance.GetText(textID, strParamArr);
            else if (strParamArr != null && tcParmaArr == null)
                return TextManager.Instance.GetText(textID, tcParmaArr);
            else if (strParamArr == null && strParamDic != null)
                return TextManager.Instance.GetText(textID, strParamDic);
            else if (strParamArr != null && tcParamDic == null)
                return TextManager.Instance.GetText(textID, tcParamDic);
            return null;
        }
    }
}