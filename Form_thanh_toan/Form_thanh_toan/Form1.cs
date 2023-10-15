using Form_thanh_toan.database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form_thanh_toan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows.Cast<DataGridViewRow>().FirstOrDefault(p => p.Cells[5].Value + "" == cmbSanPham.SelectedValue + "");
            if (row == null)
            {
                int newRow = dataGridView1.Rows.Add();
                dataGridView1.Rows[newRow].Cells[0].Value = newRow + 1;
                dataGridView1.Rows[newRow].Cells[1].Value = cmbSanPham.Text;
                dataGridView1.Rows[newRow].Cells[2].Value = txtSoLuong.Text;
                dataGridView1.Rows[newRow].Cells[3].Value = txtDonGia.Text;
                dataGridView1.Rows[newRow].Cells[4].Value = txtThanhTien.Text;
                dataGridView1.Rows[newRow].Cells[5].Value = cmbSanPham.SelectedValue;
            }
            else
            {
                row.Cells[2].Value = int.Parse(row.Cells[2].Value.ToString()) + int.Parse(txtSoLuong.Text);
                row.Cells[4].Value = decimal.Parse(txtDonGia.Text) * int.Parse(row.Cells[2].Value.ToString());
            }
            txtTotalAmout.Text = (decimal.Parse(txtThanhTien.Text) + decimal.Parse(txtTotalAmout.Text)).ToString();
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            Model1 context = new Model1();
            cmbSanPham.DataSource = context.Product.ToList();
            cmbSanPham.DisplayMember = "ProductName";
            cmbSanPham.ValueMember = "ProductId";
        }

        private void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDonGia.Text = (cmbSanPham.SelectedItem as Product).SellPrice.ToString();
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtThanhTien.Text = (decimal.Parse(txtDonGia.Text) * int.Parse(txtSoLuong.Text)).ToString();
            }
            catch
            {
                txtThanhTien.Text = "0";
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            var context = new Model1();
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoice = new Invoice();
                    invoice.InvoiceDate = DateTime.Now;
                    invoice.TotalAmount = decimal.Parse(txtTotalAmout.Text);
                    if (!string.IsNullOrEmpty(txtGhiChu.Text))
                        invoice.Note = txtGhiChu.Text;
                    context.Invoice.Add(invoice);
                    invoice.InvoiceId = context.SaveChanges();

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        InvoiceDetail detail = new InvoiceDetail()
                        {
                            InvoiceId = invoice.InvoiceId,
                            ProductId = dataGridView1.Rows[i].Cells[5].Value.ToString(),
                            Price = decimal.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString()),
                            Quantity = int.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString())
                        };
                        context.InvoiceDetail.Add(detail);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    MessageBox.Show("Thêm đơn hàng thành công");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Đã gặp lỗi, rollback!!!" + ex.Message);
                }
            }
        }

        
    }
}
