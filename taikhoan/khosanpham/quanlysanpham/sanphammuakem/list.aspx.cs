﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Database;
using DongAGroup.Helpers.Encrypt;

public partial class prodraftadmin_congty_nhanvien_EmployeeInfo : AdminPageUtils
{
    public pro_Product pro_Product { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        ASPxGridTags.DataSource = dbConnect.pro_Products.Where(n => n.IsStatus == true&& n.Id != int.Parse(Request["Id"])).OrderByDescending(n => n.Id);
        ASPxGridTags.DataBind();
        pro_Product = dbConnect.pro_Products.Single(n => n.Id == int.Parse(Request["Id"]));
        if (!IsPostBack)
        {
            LoadDanhMuc();
        }

    }
    void LoadDanhMuc()
    {
        rptDanhMuc.DataSource = dbConnect.pro_ProductRelateds.Where(n => n.pro_ProductId == int.Parse(Request["Id"]))
            .Select(n => new {
                dbConnect.pro_Products.Single(m=>m.Id==n.pro_ProductRelatedId).ProductName,
                dbConnect.pro_Products.Single(m => m.Id == n.pro_ProductRelatedId).Image,
                dbConnect.pro_Products.Single(m => m.Id == n.pro_ProductRelatedId).Code,
                n.Id,n.NumOr
            })
            .OrderBy(n => n.NumOr);
        rptDanhMuc.DataBind();

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        var pro_ProductModel = dbConnect.pro_Products.Single(n => n.Id == int.Parse(Request["Id"]));
        lblMessage.Visible = true;
        try
        {
            if (hdfId.Value == "")
            {
                List<object> lstValueTags = ASPxGridTags.GridView.GetSelectedFieldValues("Id");
                int i = 0;
                foreach (var item in lstValueTags)
                {
                    var countRelated = dbConnect.pro_ProductRelateds.Where(n => n.pro_ProductId == int.Parse(Request["Id"]) && n.pro_ProductRelatedId == int.Parse(item.ToString()));
                    if(countRelated.Count()==0)
                    {
                        var model = new pro_ProductRelated()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = Account.Employee.Email,
                            NumOr = int.Parse(txtNumOr.Text) + i,
                            IsStatus = true,
                            pro_ProductId = int.Parse(Request["Id"]),
                            pro_ProductRelatedId = int.Parse(item.ToString()),
                            IsDeleted = false
                        };
                        dbConnect.pro_ProductRelateds.InsertOnSubmit(model);
                    }
                    i++;
                }
                dbConnect.SubmitChanges();
                lblMessage.Text = "Thêm sản phẩm mua kèm thành công";
                LoadDanhMuc();
            }
            else {
                pro_ProductRelated model = dbConnect.pro_ProductRelateds.Single(m => m.Id == int.Parse(hdfId.Value));
                model.NumOr = int.Parse(txtNumOr.Text);
                model.UpdatedAt = DateTime.Now;
                model.UpdatedBy = Account.Employee.Email;
                dbConnect.SubmitChanges();
                lblMessage.Text = "Cập nhật sản phẩm mua kèm thành công";
                LoadDanhMuc();
            }
            hdfId.Value = "";
        }
        catch(Exception ex)
        {
            lblMessage.Text = "Đã xảy ra lỗi. Vui lòng kiểm tra lại dữ liệu"+ex.ToString();
        }
    }

    protected void btnDelete_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton l = sender as ImageButton;
        pro_ProductRelated model = dbConnect.pro_ProductRelateds.Single(m => m.Id == int.Parse(l.CommandArgument));
        dbConnect.pro_ProductRelateds.DeleteOnSubmit(model);
        dbConnect.SubmitChanges();
        LoadDanhMuc();
        lblMessage.Text = "Xóa sản phẩm mua kèm thành công";
    }

    protected void ImgEdit_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton l = sender as ImageButton;
        pro_ProductRelated model = dbConnect.pro_ProductRelateds.Single(m => m.Id == int.Parse(l.CommandArgument));
        txtNumOr.Text = model.NumOr.ToString();
        hdfId.Value = model.Id.ToString();
    }
}