using System;
using DryIoc;
using upendo.CrossCutting.Localization;
using upendo.Managers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace upendo.Localization
{
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		private readonly ITranslationManager translationManager;

		public string Text { get; set; }

		public TranslateExtension()
        {
			translationManager = DependencyServiceManager.Instance.Container.Resolve<ITranslationManager>();
        }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(Text))
				return string.Empty;

			return translationManager.GetResource(Text);
		}
	}
}
