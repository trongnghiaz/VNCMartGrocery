# VNCMartGrocery Front-end Implementation Specification

Tai lieu nay tong hop tu code backend hien co cua solution `VNCMartGrocery`, dung lam dac ta de xay dung front-end cho he thong VNC Mart.

## 1. Tong quan he thong

Backend la ASP.NET Core Web API theo kien truc nhieu tang:

- `VNC.Api`: controller, middleware, Swagger, CORS, JWT auth.
- `VNC.Application`: service nghiep vu, DTO/request/response model.
- `VNC.Domain`: entity va enumeration.
- `VNC.Infrastructure`: EF Core SQL Server, DbContext, JWT Bearer config.

He thong co hai khu vuc front-end chinh:

- Customer storefront: dang ky/dang nhap, xem danh muc, xem san pham, gio hang, checkout, QR thanh toan.
- Admin back-office: dang nhap nhan vien, dashboard, quan ly san pham, danh muc, don hang, khach hang, nhan vien.

Backend da cau hinh CORS cho:

- `http://localhost:5173`
- `http://localhost:8080`

Neu dung Vue/Vite, nen chay FE tai `localhost:5173`.

## 2. Base URL va Swagger

Theo `launchSettings.json`:

- HTTP: `http://localhost:5067`
- HTTPS: `https://localhost:7143`
- Swagger khi development: `/swagger`
- Health check: `GET /ping`

Khuyen nghi FE dung bien moi truong:

```env
VITE_API_BASE_URL=http://localhost:5067
```

## 3. Chuan response

Phan lon controller tra ve `ApiResponse<T>`:

```ts
export interface ApiResponse<T> {
  isSuccess: boolean
  data: T
  errorMessage?: string
  statusCode: number
}
```

Luu y JSON mac dinh cua ASP.NET Core thuong serialize camelCase, vi vay FE nen dung cac field `isSuccess`, `errorMessage`, `statusCode`.

Middleware loi nghiep vu tra ve shape khac:

```ts
export interface ErrorResponse {
  success: false
  statusCode: number
  message: string
  detail: string
}
```

Client API nen normalize loi theo thu tu:

1. Neu co `errorMessage`, hien thi `errorMessage`.
2. Neu co `message`, hien thi `message`.
3. Neu HTTP 401, logout/redirect login.
4. Neu HTTP 403, hien thi khong co quyen truy cap.
5. Con lai hien thi loi mac dinh.

## 4. Xac thuc va phan quyen

Backend dung JWT Bearer. Sau login thanh cong, luu token va gui header:

```http
Authorization: Bearer <token>
```

Ket qua login:

```ts
export interface AuthResultDto {
  id: number
  account: string
  fullName: string
  role: string
  isStaff: boolean
  token: string
}
```

Role:

- Customer login: `role = "Customer"`, `isStaff = false`.
- Staff login: `role` lay tu bang Role, route admin hien tai yeu cau `Admin`, `isStaff = true`.

Dieu huong sau login:

- `isStaff === true && role === "Admin"` -> `/admin`
- `isStaff === true` nhung role khong du quyen -> hien thi loi/quyen han
- `isStaff === false` -> `/`

Token het han sau 60 phut theo `JwtSettings.ExpiryInMinutes`.

## 5. Enum nghiep vu

Order status:

| Value | Label |
|---:|---|
| 1 | Chờ xác nhận |
| 2 | Đang xử lý |
| 3 | Đã giao hàng |
| 4 | Đã nhận hàng |
| 5 | Đã hủy |

Payment method:

| Value | Label |
|---:|---|
| 1 | COD |
| 2 | Chuyển khoản |

Payment status:

| Value | Label |
|---:|---|
| 1 | Chưa thanh toán |
| 2 | Đã thanh toán |

Front-end can de y: checkout gui `paymentMethod` la text name, khong phai value. Gia tri hop le theo code hien tai: `"COD"` hoac `"Chuyển khoản"`.

## 6. TypeScript model de xuat

### Paging

```ts
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}
```

### Product

```ts
export interface ProductDto {
  productId: number
  productCode: string
  productName: string
  slug: string
  price: number
  originalPrice?: number | null
  thumbnailUrl?: string | null
  stockQuantity: number
  ratingAverage: number
  isAvailable: boolean
  categoryName: string
}

export interface ProductDetailDto extends ProductDto {
  description?: string | null
}

export interface GetProductsRequest {
  searchTerm?: string
  pageNumber: number
  pageSize: number
}

export interface CreateProductRequest {
  productCode: string
  productName: string
  slug: string
  categoryId: number
  price: number
  originalPrice?: number | null
  description?: string | null
  thumbnailUrl?: string | null
  stockQuantity: number
}

export interface UpdateProductRequest extends CreateProductRequest {
  isVisible: boolean
}
```

### Category

```ts
export interface CategoryDto {
  categoryId: number
  categoryName: string
  slug: string
  displayOrder: number
  isVisible: boolean
  description?: string | null
}

export interface CreateCategoryRequest {
  categoryName: string
  slug: string
  displayOrder: number
  isVisible: boolean
  description?: string | null
}
```

### Cart

```ts
export interface CartDto {
  cartId: number
  customerId: number
  updatedAt: string
  items: CartItemDto[]
  totalQuantity: number
  subTotal: number
}

export interface CartItemDto {
  cartItemId: number
  productId: number
  productName: string
  thumbnailUrl?: string | null
  price: number
  quantity: number
  totalPrice: number
}

export interface AddToCartRequest {
  productId: number
  quantity: number
}
```

Canh bao hien tai: `CartService.MapToDto` chua set `customerId` va `updatedAt`, nen hai field nay co the nhan gia tri mac dinh (`0`, default date) tuy serializer. FE khong nen phu thuoc vao hai field do cho den khi backend bo sung mapping.

### Checkout

```ts
export interface CreateOrderDto {
  receiverName: string
  receiverPhone: string
  shippingAddress: string
  paymentMethod: 'COD' | 'Chuyển khoản'
  note?: string | null
  items: Array<{
    productId: number
    quantity: number
  }>
}

export interface QrPaymentResultDto {
  qrCodeUrl: string
  qrDataString: string
  orderCode: string
  totalAmount: number
}
```

### Admin

```ts
export interface DashboardSummaryDto {
  totalRevenue: number
  totalOrders: number
  todayOrders: number
  pendingOrders: number
  totalCustomers: number
  totalProducts: number
  lowStockProducts: number
}

export interface RevenueChartDto {
  date: string
  revenue: number
  orderCount: number
}

export interface TopProductDto {
  productId: number
  productName: string
  thumbnailUrl?: string | null
  totalQuantitySold: number
  totalRevenue: number
}

export interface RecentOrderDto {
  orderId: number
  orderCode: string
  customerId: number
  customerName?: string | null
  orderDate: string
  orderStatus: string
  paymentStatus: string
  totalPayAmount: number
}

export interface AdminOrderDto {
  orderId: number
  orderCode: string
  customerId: number
  customerName?: string | null
  orderDate: string
  orderStatus: string
  paymentMethod: string
  paymentStatus: string
  totalOriginalAmount: number
  discountAmount: number
  shippingFee: number
  totalPayAmount: number
}

export interface AdminOrderDetailDto extends AdminOrderDto {
  receiverName: string
  receiverPhone: string
  shippingAddress: string
  note?: string | null
  items: AdminOrderItemDto[]
}

export interface AdminOrderItemDto {
  orderItemId: number
  productId: number
  productName: string
  price: number
  quantity: number
  amount: number
}

export interface AdminCustomerDto {
  customerId: number
  phoneNumber: string
  fullName?: string | null
  email?: string | null
  isActive: boolean
  createdAt: string
  totalOrders: number
  totalSpent: number
}

export interface AdminCustomerDetailDto extends AdminCustomerDto {
  gender?: string | null
  dateOfBirth?: string | null
  zaloId?: string | null
  addresses: AdminCustomerAddressDto[]
}

export interface AdminCustomerAddressDto {
  customerAddressId: number
  receiverName: string
  receiverPhone: string
  fullAddress: string
  isDefault: boolean
}

export interface AdminStaffDto {
  staffId: number
  fullName: string
  email: string
  isActive: boolean
  createdAt: string
  roleId: number
  roleName: string
}
```

## 7. API contract

### Auth

| Method | Endpoint | Auth | Body/Query | Response |
|---|---|---|---|---|
| POST | `/api/Auth/customer/register` | Public | `RegisterRequest` | `ApiResponse<boolean>` |
| POST | `/api/Auth/customer/login` | Public | `{ account, password }` where account = phone | `ApiResponse<AuthResultDto>` |
| POST | `/api/Auth/staff/login` | Public | `{ account, password }` where account = email | `ApiResponse<AuthResultDto>` |

Validation:

- Register phone required, max 15.
- Password required, 6-100 chars.
- Email optional but must be valid.
- Duplicate phone returns bad request: `Số điện thoại này đã được sử dụng trên hệ thống.`

### Public catalog

| Method | Endpoint | Auth | Query | Response |
|---|---|---|---|---|
| GET | `/api/Category` | Public | none | `ApiResponse<CategoryDto[]>` |
| GET | `/api/Category/{id}` | Public | none | `ApiResponse<CategoryDto>` |
| GET | `/api/Product` | Public | `searchTerm`, `pageNumber`, `pageSize` | `ApiResponse<PagedResult<ProductDto>>` |
| GET | `/api/Product/{id}` | Public | none | `ApiResponse<ProductDetailDto>` |

Product public chi tra san pham `IsVisible = true`. Search theo `productName` hoac `productCode`, sort moi nhat truoc.

Luu y hien tai public product chua co query theo `categoryId`, `slug`, min/max price. Neu FE can category filter that su, backend can bo sung sau.

### Cart

Tat ca cart endpoint can JWT.

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| GET | `/api/Cart` | none | `ApiResponse<CartDto>` |
| POST | `/api/Cart/add` | `{ productId, quantity }` | `ApiResponse<CartDto>` |
| PUT | `/api/Cart/items/{productId}` | query `quantity` | `ApiResponse<CartDto>` |
| DELETE | `/api/Cart/items/{productId}` | none | `ApiResponse<CartDto>` |

Behavior:

- `GET /api/Cart` tu tao gio rong neu customer chua co cart.
- Add neu san pham da co thi cong don quantity.
- Update `quantity <= 0` se xoa item.
- Backend hien tai chua validate ton kho khi add/update cart; ton kho duoc validate o checkout.

### Customer order

Tat ca order endpoint can JWT.

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| POST | `/api/Order/checkout` | `CreateOrderDto` | `ApiResponse<string>` orderCode |
| GET | `/api/Order/{orderCode}/qr-payment` | none | `ApiResponse<QrPaymentResultDto>` |

Checkout behavior:

- FE gui danh sach item `{ productId, quantity }`.
- Backend group item trung productId, validate product ton tai, dang ban, du ton kho.
- Backend tru ton kho, tao order, xoa cac cart item da checkout.
- Order moi mac dinh: `OrderStatus = Đang xử lý`, `PaymentStatus = Chưa thanh toán`.
- Order code format: `VNC-CS1-yyyyMMdd-0001`.
- Tong tien hien tai = tong gia san pham, chua tinh voucher/shipping.

QR payment:

- Lay QR theo `orderCode`.
- `qrCodeUrl` la link anh VietQR, FE co the render truc tiep bang `<img>`.

### Admin dashboard

Tat ca admin endpoint can JWT role `Admin`.

| Method | Endpoint | Query | Response |
|---|---|---|---|
| GET | `/api/admin/dashboard/summary` | `lowStockThreshold=10` | `ApiResponse<DashboardSummaryDto>` |
| GET | `/api/admin/dashboard/revenue` | `fromDate`, `toDate` | `ApiResponse<RevenueChartDto[]>` |
| GET | `/api/admin/dashboard/top-products` | `top=5` | `ApiResponse<TopProductDto[]>` |
| GET | `/api/admin/dashboard/recent-orders` | `take=10` | `ApiResponse<RecentOrderDto[]>` |

Mac dinh chart revenue lay 30 ngay gan nhat neu khong truyen date.

### Admin products

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| GET | `/api/admin/products` | `searchTerm`, `pageNumber`, `pageSize` | `ApiResponse<PagedResult<ProductDto>>` |
| GET | `/api/admin/products/{id}` | none | `ApiResponse<ProductDetailDto>` |
| POST | `/api/admin/products` | `CreateProductRequest` | `ApiResponse<ProductDetailDto>` |
| PUT | `/api/admin/products/{id}` | `UpdateProductRequest` | `ApiResponse<boolean>` |
| DELETE | `/api/admin/products/{id}` | none | `ApiResponse<boolean>` |

Delete la soft delete: backend set `IsVisible = false`.

Canh bao hien tai: `GET /api/admin/products` co the tra paging sai `totalCount/totalPages` vi service da page truoc roi lai goi `PagedResult.CreateAsync` tren list da page. FE nen uu tien hien thi `items`, va nen sua backend neu can pagination admin chuan.

### Admin categories

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| GET | `/api/admin/categories` | none | `ApiResponse<CategoryDto[]>` |
| GET | `/api/admin/categories/{id}` | none | `ApiResponse<CategoryDto>` |
| POST | `/api/admin/categories` | `CreateCategoryRequest` | `ApiResponse<CategoryDto>` |
| PUT | `/api/admin/categories/{id}` | `CreateCategoryRequest` | `ApiResponse<boolean>` |
| DELETE | `/api/admin/categories/{id}` | none | `ApiResponse<boolean>` |

Delete la soft delete: backend set `IsVisible = false`.

### Admin orders

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| GET | `/api/admin/orders` | `searchTerm`, `orderStatusValue`, `paymentStatusValue`, `fromDate`, `toDate`, `pageNumber`, `pageSize` | `ApiResponse<PagedResult<AdminOrderDto>>` |
| GET | `/api/admin/orders/{id}` | none | `ApiResponse<AdminOrderDetailDto>` |
| PUT | `/api/admin/orders/{id}/status` | query `statusValue` | `ApiResponse<boolean>` |
| PUT | `/api/admin/orders/{id}/payment-status` | query `paymentStatusValue` | `ApiResponse<boolean>` |

Valid transition:

- `Chờ xác nhận` -> `Đang xử lý` hoac `Đã hủy`
- `Đang xử lý` -> `Đã giao hàng` hoac `Đã hủy`
- `Đã giao hàng` -> `Đã nhận hàng`
- `Đã nhận hàng`: khong duoc doi nua
- `Đã hủy`: khong duoc doi nua

Neu chuyen sang `Đã hủy`, backend tra lai ton kho mot lan.

### Admin customers

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| GET | `/api/admin/customers` | `searchTerm`, `isActive`, `pageNumber`, `pageSize` | `ApiResponse<PagedResult<AdminCustomerDto>>` |
| GET | `/api/admin/customers/{id}` | none | `ApiResponse<AdminCustomerDetailDto>` |
| PUT | `/api/admin/customers/{id}/lock` | none | `ApiResponse<boolean>` |
| PUT | `/api/admin/customers/{id}/unlock` | none | `ApiResponse<boolean>` |

Search theo phone, full name, email.

### Admin staffs

| Method | Endpoint | Body/Query | Response |
|---|---|---|---|
| GET | `/api/admin/staffs` | none | `ApiResponse<AdminStaffDto[]>` |
| GET | `/api/admin/staffs/{id}` | none | `ApiResponse<AdminStaffDto>` |
| PUT | `/api/admin/staffs/{id}/lock` | none | `ApiResponse<boolean>` |
| PUT | `/api/admin/staffs/{id}/unlock` | none | `ApiResponse<boolean>` |

Hien tai backend chua co endpoint tao/sua nhan vien.

## 8. Route map front-end de xuat

### Customer routes

- `/`: trang chu / product listing.
- `/products`: danh sach san pham, query search/page.
- `/products/:id` hoac `/products/:slug`: chi tiet san pham. Backend hien tai get by `id`, neu route theo slug thi FE can map slug -> id tu list hoac backend bo sung endpoint slug.
- `/categories/:slug`: trang danh muc. Backend hien tai chua co filter theo category, nen ban dau co the loc client-side tren trang list da tai, hoac bo sung API.
- `/cart`: gio hang, can login.
- `/checkout`: thong tin nhan hang va thanh toan, can login.
- `/checkout/success/:orderCode`: ket qua dat hang, hien QR neu payment method la chuyen khoan.
- `/login`: customer login.
- `/register`: customer register.
- `/account`: thong tin tai khoan. Backend hien tai chua co endpoint profile, nen co the hien thong tin tu token/login result.

### Admin routes

- `/admin/login`: staff login.
- `/admin`: dashboard.
- `/admin/products`: list/search product.
- `/admin/products/new`: create product.
- `/admin/products/:id`: detail/edit product.
- `/admin/categories`: category management.
- `/admin/orders`: order list/filter.
- `/admin/orders/:id`: order detail, update status/payment.
- `/admin/customers`: customer list/filter.
- `/admin/customers/:id`: customer detail, lock/unlock.
- `/admin/staffs`: staff list, lock/unlock.

## 9. Trang va chuc nang can xay dung

### Customer storefront

Home/product listing:

- Lay category bang `GET /api/Category`.
- Lay product bang `GET /api/Product`.
- Hien search box theo `searchTerm`.
- Pagination theo `pageNumber`, `pageSize`.
- Card san pham: thumbnail, ten, category, gia, gia goc, rating, stock badge, nut add to cart.
- Neu `stockQuantity <= 0` hoac `isAvailable === false`, disable add button.

Product detail:

- Lay `GET /api/Product/{id}`.
- Hien description, price, original price, stock, quantity selector.
- Add to cart can login; neu chua login redirect `/login?redirect=...`.

Cart:

- Lay `GET /api/Cart`.
- Update quantity qua `PUT /api/Cart/items/{productId}?quantity=n`.
- Remove qua `DELETE /api/Cart/items/{productId}`.
- Nen debounce khi user sua quantity.
- Vi ton kho chi validate o checkout, nen truoc checkout nen canh bao neu quantity > stock neu FE co stock tu product detail/list.

Checkout:

- Lay cart, map `cart.items` sang `CreateOrderDto.items`.
- Form receiver:
  - `receiverName` required, max 100.
  - `receiverPhone` required, max 15.
  - `shippingAddress` required, max 500.
  - `paymentMethod` required: `"COD"` hoac `"Chuyển khoản"`.
  - `note` optional.
- Submit `POST /api/Order/checkout`.
- Neu thanh cong, backend tra `orderCode`.
- Neu payment method la `"Chuyển khoản"`, goi `GET /api/Order/{orderCode}/qr-payment` va hien QR.

### Admin dashboard

- Summary cards: revenue, total orders, today orders, pending orders, customers, products, low stock.
- Revenue chart theo date range.
- Top products chart/table.
- Recent orders table.

### Admin product management

- List/search/pagination.
- Form create/update:
  - `productCode` required, max 50.
  - `productName` required, max 200.
  - `slug` required, max 250.
  - `categoryId` required.
  - `price >= 0`.
  - `originalPrice >= 0`.
  - `thumbnailUrl` max 500.
  - `stockQuantity >= 0`.
  - `isVisible` khi update.
- Thumbnail hien tai la URL string, backend chua co upload image.
- Delete nen hien confirm vi la soft delete/an san pham.

### Admin category management

- Table/list category sorted theo `displayOrder`.
- Form:
  - `categoryName` required, max 100.
  - `slug` required, max 150.
  - `displayOrder` number.
  - `isVisible` boolean.
  - `description` optional.

### Admin order management

- Filters:
  - search term.
  - order status value.
  - payment status value.
  - from/to date.
- Table columns:
  - order code, customer, order date, order status, payment method, payment status, total.
- Detail:
  - receiver info, shipping address, note, items, totals.
  - status update select chi hien cac transition hop le.
  - payment status update select: unpaid/paid.

### Admin customer management

- Filters: search, active status.
- Detail: info, addresses, total orders, total spent.
- Lock/unlock actions.

### Admin staff management

- List staff.
- Lock/unlock actions.
- Chua co create/edit API.

## 10. State management de xuat

Nen tach store:

- `authStore`
  - `user: AuthResultDto | null`
  - `token`
  - `isAuthenticated`
  - `isAdmin`
  - `loginCustomer`, `loginStaff`, `register`, `logout`
- `cartStore`
  - `cart: CartDto | null`
  - `fetchCart`, `addItem`, `updateQuantity`, `removeItem`, `clearLocal`
- `catalogStore`
  - categories cache
  - product list params/result
- `adminStore` hoac tung domain store:
  - dashboard, products, orders, customers, categories, staffs

Nen luu token trong `localStorage` de refresh page khong mat session. Neu yeu cau bao mat cao hon, can backend ho tro httpOnly cookie, hien tai chua co.

## 11. API client de xuat

Nen dung mot wrapper Axios/fetch:

```ts
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

api.interceptors.request.use((config) => {
  const token = authStore.token
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const data = error.response?.data
    const message =
      data?.errorMessage ||
      data?.message ||
      (error.response?.status === 401 ? 'Phiên đăng nhập đã hết hạn.' : 'Có lỗi xảy ra.')

    if (error.response?.status === 401) {
      authStore.logout()
    }

    return Promise.reject(new Error(message))
  },
)
```

Neu dung TypeScript, nen generic hoa:

```ts
async function getData<T>(promise: Promise<ApiResponse<T>>): Promise<T> {
  const res = await promise
  if (!res.isSuccess) throw new Error(res.errorMessage || 'Request failed')
  return res.data
}
```

## 12. UX/UI de xuat

Customer:

- Giao dien grocery/e-commerce ro rang, uu tien tim kiem nhanh, card san pham doc duoc gia va ton kho.
- Header co search, category nav, cart badge, account menu.
- Cart drawer hoac cart page rieng deu hop.
- Checkout nen la 2 cot desktop: form nhan hang + order summary; mobile stacked.

Admin:

- Layout sidebar + topbar.
- Tables co loading, empty state, pagination, filter bar.
- Action destructive/lock/delete can confirm modal.
- Toast cho add cart, checkout, save admin.
- Badge mau rieng cho order/payment status.

Formatting:

- Tien te: `Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' })`.
- Date: `Intl.DateTimeFormat('vi-VN')`.
- Backend DateTime la local server time, FE nen hien theo locale VN.

## 13. Route guard

Customer protected:

- `/cart`
- `/checkout`
- `/account`

Admin protected:

- Tat ca `/admin/*` tru `/admin/login`.
- Dieu kien: co token, `isStaff = true`, `role = "Admin"`.

Neu khong co token:

- Customer route -> `/login?redirect=<current>`.
- Admin route -> `/admin/login?redirect=<current>`.

Neu co token nhung khong dung role:

- Hien trang `403 Forbidden`.

## 14. Nhung gioi han backend hien tai FE can biet

- Chua co endpoint xem lich su don hang cua customer.
- Chua co endpoint profile/update profile/address cho customer.
- Chua co endpoint filter product theo category, price range, slug.
- Chua co upload image; product thumbnail nhap URL.
- Chua co endpoint voucher.
- Chua co create/edit staff.
- Public category endpoint hien tra ca category an (`IsVisible = false`) vi service khong filter. FE public nen loc `isVisible === true`.
- Public product detail chi lay theo `id`, khong lay theo `slug`.
- Cart DTO mapping hien tai khong set `customerId`, `updatedAt`.
- Admin product pagination co kha nang sai `totalCount` do service page hai lan.
- `ProductDto` admin khong co `isVisible`, nen list admin khong phan biet san pham dang an neu chi dung DTO hien tai. Detail cung khong tra `isVisible`. Update van can gui `isVisible`, nen FE can mac dinh `true` hoac backend nen bo sung field.

## 15. Checklist trien khai front-end

1. Khoi tao project Vite + Vue 3 + TypeScript.
2. Cai router, state management, UI framework/chart library neu can.
3. Tao `.env` voi `VITE_API_BASE_URL`.
4. Tao API client va error normalizer.
5. Tao auth store, route guards.
6. Tao catalog services: category/product.
7. Tao cart services va cart store.
8. Tao checkout flow va QR payment view.
9. Tao admin layout va admin login.
10. Tao dashboard cards/charts.
11. Tao CRUD admin products/categories.
12. Tao admin order list/detail/status update.
13. Tao admin customer/staff list/detail/lock/unlock.
14. Kiem thu 401/403/400/500 va token expiry.
15. Kiem thu responsive mobile/desktop.

## 16. De xuat cai thien backend truoc khi FE hoan thien

Nen bo sung hoac sua cac diem sau de FE lam viec dep hon:

- Them `GET /api/Product?categoryId=&minPrice=&maxPrice=&sortBy=`.
- Them `GET /api/Product/slug/{slug}`.
- Public category nen filter `IsVisible = true`.
- Them `isVisible` vao `ProductDto` hoac tao `AdminProductDto`.
- Sua `GetAdminProductsAsync` de paging dung total count.
- Map day du `customerId`, `updatedAt` trong `CartDto`.
- Them customer order history: `GET /api/Order/my-orders`, `GET /api/Order/my-orders/{id}`.
- Them customer profile/address endpoints.
- Them upload image endpoint hoac tich hop object storage.
- Chuan hoa error response ve mot schema duy nhat.
