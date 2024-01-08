namespace Liquid.BriskUI
{
    /// <summary>
    /// The label and address information for provider.
    /// </summary>
    public struct UICatalogData
    {
        public string UILabel;
        public string UIAddress;

        public bool IsLegal
        {
            get
            {
                if (UILabel == null ||
                    UIAddress == null)
                    return false;
                return true;
            }
        }

        public UICatalogData(string label, string address)
        {
            UILabel = label;
            UIAddress = address;
        }
    }
}