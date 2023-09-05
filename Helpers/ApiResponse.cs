namespace Bantuin_ISMCXIV_Admin.Helpers
{
	public class ApiResponse
	{
		public bool IsSuccess { get; set; }
		public int StatusCode { get; set; }
		public object? Message { get; set; }
		public long Timestamp { get; set; }
		public object? Data { get; set; }
	}

	public class ResponseHelper
	{
		public ApiResponse Success(object? message, object data = null, int code = 200)
		{
			return new ApiResponse
			{
				IsSuccess = true,
				StatusCode = code,
				Message = message,
				Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
				Data = data
			};
		}
		public ApiResponse Error(object? message, int code = 400, object data = null)
		{
			return new ApiResponse
			{
				IsSuccess = false,
				StatusCode = code,
				Message = message,
				Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
				Data = data
			};
		}
	}
}
