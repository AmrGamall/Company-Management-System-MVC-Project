using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
        public string NewPassword { get; set; }

		[Required(ErrorMessage = "Confirm Password is required")]
		[Compare(nameof(NewPassword),ErrorMessage = "Confirm Password does not match Password")]
		[DataType(DataType.Password)]
		public string ConfirmNewPassword { get; set; }
	}
}
