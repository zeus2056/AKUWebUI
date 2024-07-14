using BusinessLayer.Abstract.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace AKUWebUI.ViewComponents
{
	public class DiscountComponent : ViewComponent
	{
		private readonly IEFCoreDiscountRateService _discountRateService;

		public DiscountComponent(IEFCoreDiscountRateService discountRateService)
		{
			_discountRateService = discountRateService;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var discounts = await _discountRateService.GetAllAsync();
			return View(discounts);
		}
	}
}
