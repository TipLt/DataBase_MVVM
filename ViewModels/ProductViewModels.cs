using DataBase_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataBase_MVVM.Models;

namespace DataBase_MVVM.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly ProductService _productService;
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private int _productId;
        private string _productName;
        private decimal? _unitPrice;

        public ProductViewModel()
        {
            // Thay đổi connection string của bạn ở đây
            string connectionString = "Server=.;Database=Northwind;Integrated Security=true;";
            _productService = new ProductService(connectionString);

            // Khởi tạo commands
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);

            // Load dữ liệu
            LoadProducts();
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    if (_selectedProduct != null)
                    {
                        ProductId = _selectedProduct.ProductId;
                        ProductName = _selectedProduct.ProductName;
                        UnitPrice = _selectedProduct.UnitPrice;
                    }
                }
            }
        }

        public int ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public decimal? UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                Products = new ObservableCollection<Product>(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteUpdate(object parameter)
        {
            return SelectedProduct != null && !string.IsNullOrWhiteSpace(ProductName);
        }

        private void ExecuteUpdate(object parameter)
        {
            try
            {
                var updatedProduct = new Product
                {
                    ProductId = ProductId,
                    ProductName = ProductName,
                    UnitPrice = UnitPrice
                };

                if (_productService.UpdateProduct(updatedProduct))
                {
                    SelectedProduct.ProductName = ProductName;
                    SelectedProduct.UnitPrice = UnitPrice;

                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh danh sách
                    OnPropertyChanged(nameof(Products));
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật sản phẩm!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteDelete(object parameter)
        {
            return SelectedProduct != null;
        }

        private void ExecuteDelete(object parameter)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa sản phẩm '{SelectedProduct.ProductName}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_productService.DeleteProduct(SelectedProduct.ProductId))
                    {
                        Products.Remove(SelectedProduct);
                        SelectedProduct = null;

                        MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa sản phẩm!", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
