namespace Liquid.EasyInput
{
    /// <summary>
    /// Inherit this interface to implement your own custom input type.
    /// </summary>
    public interface IInputer
    {
        /// <summary>
        /// Update inputer.
        /// </summary>
        public void OnUpdate();
    }
}