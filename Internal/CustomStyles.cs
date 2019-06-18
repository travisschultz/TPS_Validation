using MigraDoc.DocumentObjectModel;

namespace TPS_Validation.Internal
{
	internal class CustomStyles
	{
		public const string PatientName = "PatientName";
		public const string ColumnHeader = "ColumnHeader";
		public const string Table = "Table";
		public const string Hidden = "Hidden";
        public const string CellPass = "CellPass";
        public const string CellFail = "CellFail";
        public const string CellWarn = "CellWarn";

		public static void Define(Document doc)
		{
			var patientName = doc.Styles.AddStyle(PatientName, StyleNames.Normal);
			patientName.ParagraphFormat.Font.Size = 12;
			patientName.ParagraphFormat.Font.Bold = true;

			var heading1 = doc.Styles[StyleNames.Heading1];
			heading1.BaseStyle = StyleNames.Normal;
			heading1.Font.Size = 14;
			heading1.ParagraphFormat.SpaceBefore = 20;

			var heading2 = doc.Styles[StyleNames.Heading2];
			heading2.BaseStyle = StyleNames.Normal;
			heading2.ParagraphFormat.Shading.Color = Color.FromCmyk(100, 38, 0, 36);
			heading2.ParagraphFormat.Font.Color = Color.FromCmyk(0, 0, 0, 0);
			heading2.ParagraphFormat.Font.Bold = true;
			heading2.ParagraphFormat.Font.Size = 11;
			heading2.ParagraphFormat.SpaceBefore = 10;
			heading2.ParagraphFormat.LeftIndent = 15;   //Size.TableCellPadding;
			heading2.ParagraphFormat.RightIndent = 15;  //Size.TableCellPadding;
			heading2.ParagraphFormat.Borders.Distance = Size.TableCellPadding;

            var heading3 = doc.Styles[StyleNames.Heading3];
            heading3.BaseStyle = StyleNames.Normal;
            heading3.ParagraphFormat.Shading.Color = Color.FromCmyk(100, 38, 0, 15);
            heading3.ParagraphFormat.Font.Color = Color.FromCmyk(0, 0, 0, 0);
            heading3.ParagraphFormat.Font.Bold = true;
            heading3.ParagraphFormat.Font.Size = 10;
            heading3.ParagraphFormat.SpaceBefore = 10;
            heading3.ParagraphFormat.LeftIndent = 20;   //Size.TableCellPadding;
            heading3.ParagraphFormat.RightIndent = 15;  //Size.TableCellPadding;
            heading3.ParagraphFormat.Borders.Distance = Size.TableCellPadding;



            var header = doc.Styles[StyleNames.Header];
			header.Font.Size = 8;

			var footer = doc.Styles[StyleNames.Footer];
			footer.Font.Size = 8;

			var columnHeader = doc.Styles.AddStyle(ColumnHeader, StyleNames.Normal);
			columnHeader.Font.Size = 9;
			columnHeader.ParagraphFormat.Alignment = ParagraphAlignment.Left;
			columnHeader.ParagraphFormat.Font.Bold = true;
			columnHeader.ParagraphFormat.LeftIndent = Size.TableCellPadding;
			columnHeader.ParagraphFormat.RightIndent = Size.TableCellPadding;

            var cellPass = doc.Styles.AddStyle(CellPass, StyleNames.Normal);
            cellPass.Font.Size = 8;
            cellPass.Font.Color = Color.FromCmyk(100, 0, 100, 62);

            var cellFail = doc.Styles.AddStyle(CellFail, StyleNames.Normal);
            cellPass.Font.Size = 8;
            cellPass.Font.Color = Color.FromCmyk(0, 100, 96, 39);

            var cellWarn = doc.Styles.AddStyle(CellWarn, StyleNames.Normal);
            cellPass.Font.Size = 8;
            cellPass.Font.Color = Color.FromCmyk(0, 35, 100, 39);

            var table = doc.Styles.AddStyle(Table, StyleNames.Normal);
			table.Font.Size = 8;
			table.ParagraphFormat.Alignment = ParagraphAlignment.Center;

			//var hidden = doc.Styles.AddStyle(Hidden, StyleNames.Normal);
			//table.Font.Size = 1;
			//table.Font.Color = new Color(0, 0, 0, 0);
			//table.ParagraphFormat.LeftIndent = 0;
			//table.ParagraphFormat.RightIndent = 0;
		}
	}
}