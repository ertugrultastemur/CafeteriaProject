using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public class Messages
    {
        public static string MaintanceTime = "Sistem bakımda.";
        public static string ProductsListed = "Ürünler listelendi.";
        public static string ProductsListedByCategory = "Ürünler kategoriye göre getirildi.";
        public static string ProductDetailsListed = "Ürün detayları getirildi.";
        public static string ProductsListedByUnitPrice = "Ürünler birim fiyat aralığında getirildi.";
        public static string ProductListed = "Ürün getirildi.";
        public static string ProductDeleted = "Ürün silindi.";
        public static string ProductAdded = "Ürün eklendi.";
        public static string ProductCountOfCategoryError = "Bir kategoride en fazla 15 ürün olabilir.";
        public static string ProductNameExistsOfProducctsError = "Aynı isimle en fazla 1 ürün olabilir.";
        public static string CategoryCountExcededThanLimitCouldNotAddingProductsError = "Kategori sayısı 15'den fazla ise ürün eklenemez.";
        public static string CategoriesListed = "Kategoriler getirildi.";
        public static string CategoryListed = "Kategori getirildi.";
        public static string CategoryDeleted = "Kategori silindi.";
        public static string CategoryAdded = "Kategori eklendi.";
        public static string AuthorizationDenied = "Yetkiniz doğrulanamadı.";
        public static string UserSignUpSuccessfully = "Kullanıcı başarıyla kayıt oldu.";
        public static string CategoryNotFound = "Kategori bulunamadı.";

        public static string OrderAdded = "Sipariş eklendi.";
        public static string CategoryNullOrExists = "Böyle bir kategori bulunuyor.";
        public static string CategoryNotExists = "Böyle bir kategori bulunmuyor.";

        public static string OrderNotExists = "Sipariş bulunamadı.";
        public static string OrderListed = "Sipariş listelendi.";
        public static string ProductNotFound = "Ürün bulunamadı.";
        public static string UserNotFound = "Kullanıcı bulunamadı.";

        public static string PasswordError = "Parola hatalı.";

        public static string UserSignInSuccessfully = "Başarıyla giriş yapıldı.";

        public static string UserAlreadyExists = "Böyle bir kullanıcı mevcut.";

        public static string ClaimsListed = "Roller getirildi.";

        public static string AccessTokenCreated = "Access token yaratıldı.";

        public static string UserAdded = "Kullanıcı eklendi.";
        public static string UserListed = "Kullanıcılar getirildi.";
        public static string UserDeleted = "Kullanıcı silindi.";

        public static string EmailExistsError = "Böyle bir kullanıcı zaten mevcut.";

        public static string UserListedByEmail = "Kullanıcı getirildi.";

        public static string BranchAdded = "Şube eklendi.";
        public static string BranchesListed = "Şubeler listelendi.";
        public static string BranchListed = "Şube listelendi.";
        public static string MunicipalityAdded = "Belediye eklendi.";
        public static string MunicipalityDeleted = "Belediye silindi.";
        public static string MunicipalitiesAdded = "Belediye eklendi.";
        public static string MunicipalityListed = "Belediye listelendi.";  
        public static string MunicipalitiesListed = "Belediyeler listelendi.";
        public static string UsersListed = "Kullanıcılar listelendi.";
        public static string UserUpdated = "Kullanıcı güncellendi.";
        public static string DepartmentAdded = "Departman eklendi.";
        public static string BranchDeleted = "Şube silindi.";
        public static string BranchUpdated = "Şube güncellendi.";
        public static string DepartmentDeleted = "Departman silindi.";
        public static string DepartmentUpdated = "Departman güncellendi.";
        public static string CategoryUpdated = "Kategori güncellendi.";
        public static string MunicipalityUpdated = "Belediye güncellendi.";
        public static string ProductUpdated = "Ürün güncellendi.";
        public static string OrderDeleted = "Sipariş silindi.";
        public static string OrdersListed = "Siparişler listelendi.";
        public static string OrderUpdated = "Sipariş güncellendi.";

        public static string RefreshTokenCreated { get; internal set; }
        public static string OperationClaimAdded { get; internal set; }
        public static string OperationClaimAlreadyExists { get; internal set; }
    }
}
