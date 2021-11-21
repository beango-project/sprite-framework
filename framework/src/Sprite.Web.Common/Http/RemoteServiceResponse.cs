namespace Sprite.Web.Http
{
    public class RemoteServiceResponse
    {
        public RemoteServiceResponse(ServiceErrorInfo error)
        {
            Error = error;
        }

        public ServiceErrorInfo Error { get; set; }
    }
}