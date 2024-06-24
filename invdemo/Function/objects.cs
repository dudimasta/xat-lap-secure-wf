namespace xat.InvUtils
{
  using System.Collections.Generic;
  public class InvoiceEnvelope
  {

    public InvoiceEnvelope(Invoice _invoice)
    {
      this.invoice = _invoice;
      var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Invoice));
      var stringWriter = new System.IO.StringWriter();
      serializer.Serialize(stringWriter, invoice);
      invoiceInXml = stringWriter.ToString();
    }
    private Invoice invoice;
    private string invoiceInXml;
    public string FileName
    {

      get
      {
        System.DateTime now = System.DateTime.Now;
        string prefix = now.ToString("yy-MM-dd-HH-mm-ss-");
        return prefix + invoice.account_num + ".xml";
      }
    }

    public Invoice Invoice
    {
      get
      {
        return invoice;
      }
    }

    public string InvoiceInXml
    {
      get
      {
        return invoiceInXml;
      }
    }
  }

  public class Invoice
  {
    public string account_num { get; set; }
    public bool is_domestic { get; set; }

    public string invoice_id { get; set; }

    public System.DateTime invoice_date { get; set; }

    private List<InvoiceLine> invoiceLines = new List<InvoiceLine>();

    public List<InvoiceLine> InvoiceLines
    {
      get
      {
        return this.invoiceLines;
      }
    }

  }

  public class InvoiceLine
  {
    public string line_num {get; set; }
    public string item_id { get; set; }
    public double line_amount { get; set; }
    public double line_discout { get; set; }
  }

  public class InvoiceGenerator
  {
    public Invoice GenerateInvoice()
    {
      string guidbuf = System.Guid.NewGuid().ToString();

      string[] theArray = guidbuf.Split("-");


      Invoice invoice = new Invoice();
      invoice.account_num = theArray[0];
      invoice.invoice_id = theArray[1];
      string buf = theArray[2];
      // get first character
      char c = buf[0];
      invoice.is_domestic = char.IsNumber(c);
      invoice.invoice_date = System.DateTime.Now;

      // add some lines to the invoice
      List<InvoiceLine> lines = invoice.InvoiceLines;

      int linesToGen = new System.Random().Next(2, 10);

      for (int __i = 1; __i <= linesToGen; __i++)
      {

        InvoiceLine line = new()
        {
          line_num = __i.ToString(),
          item_id = "product_" + new System.Random().Next(1000, 9999).ToString(),
          line_amount = new System.Random().Next(1000, 500000)
        };

        // procent rabatu losujemy od 5 do 20 od kwoty linii
        line.line_discout = line.line_amount * new System.Random().Next(5, 20) / 100;
        lines.Add(line);
      }

      return invoice;
    }
  }
}