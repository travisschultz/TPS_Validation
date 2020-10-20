﻿using System;
using MigraDoc.DocumentObjectModel;

namespace TPS_Validation.Internal
{
	internal class HeaderAndFooter
	{
		public void Add(Section section, ViewModel vm)
		{
			AddHeader(section, vm);
			AddFooter(section, vm);
		}

		private void AddHeader(Section section, ViewModel vm)
		{
			var header = section.Headers.Primary.AddParagraph();
			header.Style = StyleNames.Header;
			header.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

			header.AddText($"TPS Validation and QA");
			header.AddTab();
			header.AddText($"");
		}

		private void AddFooter(Section section, ViewModel vm)
		{
			var footer = section.Footers.Primary.AddParagraph();
			footer.Style = StyleNames.Footer;
			footer.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

			footer.AddText($"Generated by {vm.App.CurrentUser.Name} ({vm.App.CurrentUser.Id}) at {DateTime.Now:g}");
			footer.AddTab();
			footer.AddText("Page ");
			footer.AddPageField();
			footer.AddText(" of ");
			footer.AddNumPagesField();
		}
	}
}