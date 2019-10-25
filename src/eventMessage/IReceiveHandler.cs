namespace eventMessage
{ 
    public interface IReceiveHandler 
    {
        void Oneceive (byte[] buffer);
    }
}