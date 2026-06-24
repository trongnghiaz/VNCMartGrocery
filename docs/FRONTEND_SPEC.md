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

## 8. API cases chi tiet

Quy uoc trong cac vi du:

- `BASE_URL = http://localhost:5067`
- Header auth khi can dang nhap: `Authorization: Bearer <token>`
- Cac response thanh cong thuong nam trong `data`.
- Loi validate model cua ASP.NET Core co the tra ve `400 ValidationProblemDetails` neu field sai annotation truoc khi vao controller.
- Loi nghiep vu nem exception se di qua middleware va tra `{ success, statusCode, message, detail }`.

### 8.1 `POST /api/Auth/customer/register`

Muc dich: tao tai khoan customer bang so dien thoai.

Input:

```json
{
  "phoneNumber": "0901234567",
  "password": "123456",
  "fullName": "Nguyen Van A",
  "email": "a@example.com"
}
```

Rules:

- `phoneNumber`: required, toi da 15 ky tu.
- `password`: required, 6-100 ky tu.
- `fullName`: optional, toi da 100 ky tu.
- `email`: optional, dung dinh dang email, toi da 100 ky tu.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": true,
  "errorMessage": null,
  "statusCode": 200
}
```

Case so dien thoai da ton tai `400`:

```json
{
  "isSuccess": false,
  "data": false,
  "errorMessage": "Số điện thoại này đã được sử dụng trên hệ thống.",
  "statusCode": 400
}
```

Case validate fail `400`: backend co the tra `ValidationProblemDetails`. FE nen hien loi theo field neu co `errors`, neu khong co thi hien message chung.

FE action:

- Sau register thanh cong co the redirect sang `/login` va hien toast.
- Khong auto login vi API register chi tra boolean, khong tra token.

### 8.2 `POST /api/Auth/customer/login`

Muc dich: customer dang nhap bang so dien thoai.

Input:

```json
{
  "account": "0901234567",
  "password": "123456"
}
```

Rules:

- `account`: required, la `PhoneNumber` cua customer.
- `password`: required.
- Chi login duoc customer `IsActive = true`.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "account": "0901234567",
    "fullName": "Nguyen Van A",
    "role": "Customer",
    "isStaff": false,
    "token": "<jwt>"
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case sai tai khoan/mat khau hoac bi khoa `401`:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Số điện thoại hoặc mật khẩu không chính xác.",
  "statusCode": 400
}
```

Luu y: controller tra `Unauthorized(...)` nhung `ApiResponse.Failure` default `statusCode = 400`. FE nen tin HTTP status cho auth flow, va dung `errorMessage` de hien thi.

FE action:

- Luu `data.token` va `data` vao auth store.
- Redirect ve query `redirect` neu co, mac dinh `/`.
- Cart badge co the goi `GET /api/Cart` ngay sau login.

### 8.3 `POST /api/Auth/staff/login`

Muc dich: nhan vien/admin dang nhap bang email.

Input:

```json
{
  "account": "admin@vncmart.local",
  "password": "123456"
}
```

Rules:

- `account`: required, la email staff.
- `password`: required.
- Staff phai `IsActive = true`.
- Staff phai co role hop le.
- Cac admin endpoint hien yeu cau role `"Admin"`.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "account": "admin@vncmart.local",
    "fullName": "Admin",
    "role": "Admin",
    "isStaff": true,
    "token": "<jwt>"
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case sai email/mat khau hoac inactive `401`:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Email hoặc mật khẩu không chính xác.",
  "statusCode": 400
}
```

Case staff chua co role `403`:

```json
{
  "success": false,
  "statusCode": 403,
  "message": "Tài khoản nhân viên của bạn hiện chưa được cấp quyền truy cập hệ thống. Vui lòng liên hệ Admin.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

FE action:

- Neu `isStaff = true` va `role = "Admin"` redirect `/admin`.
- Neu role khac Admin, hien trang/alert khong co quyen.

### 8.4 `GET /api/Category`

Muc dich: lay danh sach danh muc.

Input: khong co body/query.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": [
    {
      "categoryId": 1,
      "categoryName": "Rau củ",
      "slug": "rau-cu",
      "displayOrder": 1,
      "isVisible": true,
      "description": "Rau củ tươi"
    }
  ],
  "errorMessage": null,
  "statusCode": 200
}
```

Case rong `200`:

```json
{
  "isSuccess": true,
  "data": [],
  "errorMessage": null,
  "statusCode": 200
}
```

FE action:

- Public UI nen loc `isVisible === true` vi backend hien tra ca danh muc an.
- Sort da duoc backend sort theo `displayOrder` tang dan.

### 8.5 `GET /api/Category/{id}`

Muc dich: lay chi tiet mot danh muc theo id.

Input path:

- `id`: integer, vi du `/api/Category/1`.

Case thanh cong `200`: `ApiResponse<CategoryDto>`.

Case khong tim thay `404`:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Không tìm thấy danh mục.",
  "statusCode": 400
}
```

Luu y: HTTP status la `404`, nhung body `statusCode` co the la `400` do `Failure` default. FE nen uu tien HTTP status.

FE action:

- Neu `404`, hien not found hoac quay ve category list.

### 8.6 `GET /api/Product`

Muc dich: lay danh sach san pham public dang hien thi.

Input query:

```http
GET /api/Product?searchTerm=sua&pageNumber=1&pageSize=12
```

Rules:

- `searchTerm`: optional, tim theo `productName` hoac `productCode`.
- `pageNumber`: mac dinh 1, backend clamp min 1.
- `pageSize`: mac dinh 10, backend clamp 1-100.
- Chi tra product `IsVisible = true`.

Case thanh cong co data `200`:

```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "productId": 10,
        "productCode": "MILK001",
        "productName": "Sữa tươi",
        "slug": "sua-tuoi",
        "price": 28000,
        "originalPrice": 32000,
        "thumbnailUrl": "https://example.com/milk.jpg",
        "stockQuantity": 20,
        "ratingAverage": 5.0,
        "isAvailable": true,
        "categoryName": "Sữa"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 12,
    "totalPages": 1
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case khong co ket qua `200`:

```json
{
  "isSuccess": true,
  "data": {
    "items": [],
    "totalCount": 0,
    "pageNumber": 1,
    "pageSize": 12,
    "totalPages": 0
  },
  "errorMessage": null,
  "statusCode": 200
}
```

FE action:

- Hien empty state khi `items.length === 0`.
- Disable add cart khi `isAvailable === false` hoac `stockQuantity <= 0`.
- Neu user nhap pageSize > 100, backend tu clamp; FE nen gioi han UI tu dau.

### 8.7 `GET /api/Product/{id}`

Muc dich: lay chi tiet san pham public theo id.

Input path:

- `id`: integer, vi du `/api/Product/10`.

Case thanh cong `200`: `ApiResponse<ProductDetailDto>`.

Case khong tim thay hoac product dang an `404`:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Không tìm thấy sản phẩm.",
  "statusCode": 400
}
```

FE action:

- Neu route FE dung slug, can co `productId` tu list truoc hoac backend bo sung endpoint slug.
- Khi add cart, FE gui `productId`, khong gui slug.

### 8.8 `GET /api/Cart`

Muc dich: lay gio hang cua customer hien tai.

Auth: required customer JWT.

Input: khong co body/query.

Case thanh cong gio rong `200`:

```json
{
  "isSuccess": true,
  "data": {
    "cartId": 1,
    "customerId": 0,
    "updatedAt": "0001-01-01T00:00:00",
    "items": [],
    "totalQuantity": 0,
    "subTotal": 0
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case thanh cong co item `200`:

```json
{
  "isSuccess": true,
  "data": {
    "cartId": 1,
    "customerId": 0,
    "updatedAt": "0001-01-01T00:00:00",
    "items": [
      {
        "cartItemId": 5,
        "productId": 10,
        "productName": "Sữa tươi",
        "thumbnailUrl": "https://example.com/milk.jpg",
        "price": 28000,
        "quantity": 2,
        "totalPrice": 56000
      }
    ],
    "totalQuantity": 2,
    "subTotal": 56000
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case chua login/token sai `401`: backend JWT middleware tra unauthorized, FE logout/redirect login.

Case token hop le nhung khong lay duoc user id `401`:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Không thể xác định danh tính khách hàng.",
  "statusCode": 400
}
```

FE action:

- Khong dung `customerId`/`updatedAt` hien tai cho UI quan trong vi backend chua map dung.
- Dung `items`, `totalQuantity`, `subTotal`.

### 8.9 `POST /api/Cart/add`

Muc dich: them san pham vao gio.

Auth: required customer JWT.

Input:

```json
{
  "productId": 10,
  "quantity": 2
}
```

Rules:

- `productId`: required.
- `quantity`: required, >= 1.
- Neu product da co trong gio, backend cong don quantity.
- Backend hien chua check product ton tai/visible/ton kho o luc add; loi co the xay ra neu product id khong hop le khi map cart.

Case thanh cong `200`: tra `ApiResponse<CartDto>` moi nhat.

Case quantity < 1 `400`: validate fail.

Case chua login `401`: redirect login.

FE action:

- Sau thanh cong update cart store bang `data`.
- Nen disable add button voi product het hang tu product DTO.

### 8.10 `PUT /api/Cart/items/{productId}?quantity={quantity}`

Muc dich: cap nhat so luong item trong gio.

Auth: required customer JWT.

Input path/query:

```http
PUT /api/Cart/items/10?quantity=3
```

Rules:

- `productId`: integer.
- `quantity > 0`: set so luong moi.
- `quantity <= 0`: xoa item khoi gio.

Case thanh cong `200`: tra `ApiResponse<CartDto>` sau cap nhat.

Case cart khong ton tai `400` middleware:

```json
{
  "success": false,
  "statusCode": 500,
  "message": "Không tìm thấy giỏ hàng.",
  "detail": "Vui lòng liên hệ Admin hệ thống để biết thêm chi tiết."
}
```

Luu y: exception message nay khong nam trong danh sach middleware map 400, nen co the thanh 500. FE nen hien message than thien va cho user reload cart.

FE action:

- Khi user bam minus ve 0, co the goi PUT quantity=0 hoac DELETE endpoint.
- Nen optimistic update nhe, nhung rollback neu API loi.

### 8.11 `DELETE /api/Cart/items/{productId}`

Muc dich: xoa mot item khoi gio.

Auth: required customer JWT.

Input path:

```http
DELETE /api/Cart/items/10
```

Case thanh cong `200`: tra `ApiResponse<CartDto>` sau khi xoa.

Case item khong ton tai: service van tra cart hien tai, khong loi.

Case chua login `401`: redirect login.

FE action:

- Sau thanh cong thay cart store bang response `data`.
- Confirm la optional; voi gio hang co the xoa truc tiep va cho undo toast neu muon.

### 8.12 `POST /api/Order/checkout`

Muc dich: tao don hang tu danh sach item FE gui len.

Auth: required customer JWT.

Input:

```json
{
  "receiverName": "Nguyen Van A",
  "receiverPhone": "0901234567",
  "shippingAddress": "123 Le Loi, Quan 1, TP.HCM",
  "paymentMethod": "Chuyển khoản",
  "note": "Giao buổi sáng",
  "items": [
    {
      "productId": 10,
      "quantity": 2
    }
  ]
}
```

Rules:

- `receiverName`: required, max 100.
- `receiverPhone`: required, max 15.
- `shippingAddress`: required, max 500.
- `paymentMethod`: required, max 50, phai match enum name: `"COD"` hoac `"Chuyển khoản"`.
- `items`: required, toi thieu 1 item.
- Moi item `quantity > 0`.
- Backend group cac item trung `productId`.
- Backend tru ton kho, tao order, xoa cac cart item da checkout.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": "VNC-CS1-20260624-0001",
  "errorMessage": null,
  "statusCode": 200
}
```

Case product khong ton tai `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Sản phẩm với ID 10 không tồn tại.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

Case product ngung ban `500` theo middleware hien tai:

```json
{
  "success": false,
  "statusCode": 500,
  "message": "Sản phẩm Sữa tươi hiện không còn được bán.",
  "detail": "Vui lòng liên hệ Admin hệ thống để biết thêm chi tiết."
}
```

Case khong du ton kho `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Sản phẩm Sữa tươi không đủ tồn kho.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

Case ton kho doi do concurrency `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Tồn kho vừa thay đổi, vui lòng kiểm tra giỏ hàng và thử lại.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

Case payment method sai `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Possible values for PaymentMethodEnum: COD,Chuyển khoản",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

FE action:

- Sau checkout thanh cong, lay `orderCode = data`.
- Goi lai `GET /api/Cart` de refresh cart.
- Neu `paymentMethod = "Chuyển khoản"`, redirect `/checkout/success/:orderCode` va fetch QR.
- Neu loi ton kho, refresh product/cart va yeu cau user kiem tra lai.

### 8.13 `GET /api/Order/{orderCode}/qr-payment`

Muc dich: tao link anh QR VietQR cho don hang.

Auth: required customer JWT.

Input path:

```http
GET /api/Order/VNC-CS1-20260624-0001/qr-payment
```

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": {
    "qrCodeUrl": "https://img.vietqr.io/image/970415-102874635241-compact2.jpg?amount=56000&addInfo=THANH%20TOAN%20DON%20HANG%20VNC-CS1-20260624-0001&accountName=NGUYEN%20VAN%20NGHIA",
    "qrDataString": "https://img.vietqr.io/image/970415-102874635241-compact2.jpg?amount=56000&addInfo=THANH%20TOAN%20DON%20HANG%20VNC-CS1-20260624-0001&accountName=NGUYEN%20VAN%20NGHIA",
    "orderCode": "VNC-CS1-20260624-0001",
    "totalAmount": 56000
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case orderCode khong ton tai `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Không tìm thấy đơn hàng với mã VNC-CS1-20260624-9999.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

FE action:

- Render `<img :src="qrCodeUrl">`.
- Hien `orderCode`, `totalAmount`, bank account info neu can hardcode tu business config hoac them API config sau.

### 8.14 `GET /api/admin/dashboard/summary`

Muc dich: lay cac KPI tong quan.

Auth: required JWT role `Admin`.

Input query:

```http
GET /api/admin/dashboard/summary?lowStockThreshold=10
```

Rules:

- `lowStockThreshold`: optional, default 10.
- `totalRevenue`: sum order khong bi huy.
- `pendingOrders`: dem order status `Chờ xác nhận`.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": {
    "totalRevenue": 12500000,
    "totalOrders": 120,
    "todayOrders": 5,
    "pendingOrders": 3,
    "totalCustomers": 80,
    "totalProducts": 240,
    "lowStockProducts": 8
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case khong co token `401`: redirect `/admin/login`.

Case token khong phai Admin `403`: hien forbidden.

FE action:

- Format money VND.
- Cho phep admin doi nguong low stock neu UI can.

### 8.15 `GET /api/admin/dashboard/revenue`

Muc dich: lay doanh thu theo ngay cho chart.

Auth: Admin.

Input query:

```http
GET /api/admin/dashboard/revenue?fromDate=2026-06-01&toDate=2026-06-24
```

Rules:

- `fromDate`: optional, neu khong truyen lay 30 ngay gan nhat.
- `toDate`: optional, backend tinh inclusive theo ngay bang cach cong 1 ngay va so sanh `<`.
- Khong tinh order da huy.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": [
    {
      "date": "2026-06-24T00:00:00",
      "revenue": 560000,
      "orderCount": 8
    }
  ],
  "errorMessage": null,
  "statusCode": 200
}
```

FE action:

- Neu ngay nao khong co order, backend khong tra point; FE co the fill missing dates bang revenue 0 de chart lien mach.

### 8.16 `GET /api/admin/dashboard/top-products`

Muc dich: lay top san pham ban chay.

Auth: Admin.

Input query:

```http
GET /api/admin/dashboard/top-products?top=5
```

Rules:

- `top`: optional, default 5, backend clamp 1-50.
- Khong tinh order da huy.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": [
    {
      "productId": 10,
      "productName": "Sữa tươi",
      "thumbnailUrl": null,
      "totalQuantitySold": 120,
      "totalRevenue": 3360000
    }
  ],
  "errorMessage": null,
  "statusCode": 200
}
```

FE action:

- `thumbnailUrl` hien co the null do query service khong select thumbnail tu product.
- Hien table hoac bar chart theo `totalQuantitySold`.

### 8.17 `GET /api/admin/dashboard/recent-orders`

Muc dich: lay don hang moi nhat tren dashboard.

Auth: Admin.

Input query:

```http
GET /api/admin/dashboard/recent-orders?take=10
```

Rules:

- `take`: optional, default 10, backend clamp 1-50.

Case thanh cong `200`: `ApiResponse<RecentOrderDto[]>`.

FE action:

- Click row sang `/admin/orders/{orderId}`.
- Badge status theo `orderStatus`, `paymentStatus`.

### 8.18 `GET /api/admin/products`

Muc dich: lay danh sach san pham cho admin.

Auth: Admin.

Input query:

```http
GET /api/admin/products?searchTerm=sua&pageNumber=1&pageSize=10
```

Case thanh cong `200`: `ApiResponse<PagedResult<ProductDto>>`.

Case search khong co ket qua `200`: `items = []`.

FE action:

- Luu y paging hien co kha nang sai do backend page hai lan.
- DTO hien khong co `isVisible`, nen admin list khong biet san pham dang an. Can backend bo sung `AdminProductDto` neu UI can filter an/hien.

### 8.19 `GET /api/admin/products/{id}`

Muc dich: lay chi tiet san pham bat ke visible hay hidden.

Auth: Admin.

Input path: `id`.

Case thanh cong `200`: `ApiResponse<ProductDetailDto>`.

Case khong tim thay `404`:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Không tìm thấy sản phẩm.",
  "statusCode": 400
}
```

FE action:

- Form edit can prefill cac field co trong DTO.
- `isVisible` khong co trong detail DTO; neu update ma khong co field nay, FE phai chon default, tot nhat la backend bo sung.

### 8.20 `POST /api/admin/products`

Muc dich: tao san pham moi.

Auth: Admin.

Input:

```json
{
  "productCode": "MILK001",
  "productName": "Sữa tươi",
  "slug": "sua-tuoi",
  "categoryId": 1,
  "price": 28000,
  "originalPrice": 32000,
  "description": "Sữa tươi tiệt trùng",
  "thumbnailUrl": "https://example.com/milk.jpg",
  "stockQuantity": 20
}
```

Rules:

- `productCode`: required, max 50.
- `productName`: required, max 200.
- `slug`: required, max 250.
- `categoryId`: required.
- `price >= 0`.
- `originalPrice >= 0` neu co.
- `thumbnailUrl`: optional, max 500.
- `stockQuantity >= 0`.
- Product moi luon `isVisible = true`.

Case thanh cong `201`:

```json
{
  "isSuccess": true,
  "data": {
    "productId": 10,
    "productCode": "MILK001",
    "productName": "Sữa tươi",
    "slug": "sua-tuoi",
    "price": 28000,
    "originalPrice": 32000,
    "thumbnailUrl": "https://example.com/milk.jpg",
    "stockQuantity": 20,
    "ratingAverage": 5.0,
    "isAvailable": true,
    "categoryName": "Sữa",
    "description": "Sữa tươi tiệt trùng"
  },
  "errorMessage": null,
  "statusCode": 201
}
```

Case validation fail `400`: hien loi theo field.

Case categoryId khong ton tai: co the loi database/foreign key `500`.

FE action:

- Nen load category list truoc de user chon `categoryId` hop le.
- Tao slug tu ten san pham nhung cho user sua.

### 8.21 `PUT /api/admin/products/{id}`

Muc dich: cap nhat san pham.

Auth: Admin.

Input:

```json
{
  "productCode": "MILK001",
  "productName": "Sữa tươi mới",
  "slug": "sua-tuoi-moi",
  "categoryId": 1,
  "price": 30000,
  "originalPrice": 35000,
  "description": "Mo ta moi",
  "thumbnailUrl": "https://example.com/milk-new.jpg",
  "stockQuantity": 30,
  "isVisible": true
}
```

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": true,
  "errorMessage": null,
  "statusCode": 200
}
```

Case product khong ton tai `404`:

```json
{
  "isSuccess": false,
  "data": false,
  "errorMessage": "Không tìm thấy sản phẩm để cập nhật.",
  "statusCode": 400
}
```

FE action:

- Sau save thanh cong, fetch lai detail/list.
- Can canh bao ve `isVisible` do API detail hien khong tra field nay.

### 8.22 `DELETE /api/admin/products/{id}`

Muc dich: an san pham bang soft delete.

Auth: Admin.

Input path: `id`.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy sản phẩm để xóa.`

FE action:

- Hien confirm truoc khi xoa.
- Sau thanh cong remove row khoi list hoac refetch.

### 8.23 `GET /api/admin/categories`

Muc dich: lay toan bo danh muc cho admin.

Auth: Admin.

Input: none.

Case thanh cong `200`: `ApiResponse<CategoryDto[]>`.

FE action:

- Admin nen hien ca category hidden voi badge `isVisible`.
- Co the dung chung data cho product form category select, nhung nen canh bao neu chon category hidden.

### 8.24 `GET /api/admin/categories/{id}`

Muc dich: lay chi tiet danh muc.

Auth: Admin.

Input path: `id`.

Case thanh cong `200`: `ApiResponse<CategoryDto>`.

Case khong tim thay `404`: errorMessage `Không tìm thấy danh mục.`

### 8.25 `POST /api/admin/categories`

Muc dich: tao danh muc.

Auth: Admin.

Input:

```json
{
  "categoryName": "Sữa",
  "slug": "sua",
  "displayOrder": 1,
  "isVisible": true,
  "description": "Các sản phẩm sữa"
}
```

Rules:

- `categoryName`: required, max 100.
- `slug`: required, max 150.
- `displayOrder`: integer.
- `isVisible`: boolean.
- `description`: optional.

Case thanh cong `201`: `ApiResponse<CategoryDto>`.

Case validation fail `400`: hien loi theo field.

FE action:

- Tao slug tu category name.
- Sau create refetch categories.

### 8.26 `PUT /api/admin/categories/{id}`

Muc dich: cap nhat danh muc.

Auth: Admin.

Input: giong `CreateCategoryRequest`.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy danh mục để cập nhật.`

### 8.27 `DELETE /api/admin/categories/{id}`

Muc dich: soft delete danh muc bang cach set `IsVisible = false`.

Auth: Admin.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy danh mục để xóa.`

FE action:

- Nen goi la "Ẩn danh mục" thay vi "Xóa vĩnh viễn" de dung nghiep vu.

### 8.28 `GET /api/admin/orders`

Muc dich: lay danh sach don hang admin co filter.

Auth: Admin.

Input query:

```http
GET /api/admin/orders?searchTerm=VNC&orderStatusValue=2&paymentStatusValue=1&fromDate=2026-06-01&toDate=2026-06-24&pageNumber=1&pageSize=10
```

Rules:

- `searchTerm`: search order code, receiver name, receiver phone, customer full name.
- `orderStatusValue`: optional, dung enum value 1-5.
- `paymentStatusValue`: optional, dung enum value 1-2.
- `fromDate`, `toDate`: optional, format ISO date.
- `pageNumber`: clamp min 1.
- `pageSize`: clamp 1-100.

Case thanh cong `200`: `ApiResponse<PagedResult<AdminOrderDto>>`.

Case enum value khong hop le `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Possible values for OrderStatusEnum: Chờ xác nhận,Đang xử lý,Đã giao hàng,Đã nhận hàng,Đã hủy",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

FE action:

- Select filter phai dung enum value hop le.
- Date picker nen gui `YYYY-MM-DD`.

### 8.29 `GET /api/admin/orders/{id}`

Muc dich: lay chi tiet don hang.

Auth: Admin.

Input path: `orderId`.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": {
    "orderId": 1,
    "orderCode": "VNC-CS1-20260624-0001",
    "customerId": 1,
    "customerName": "Nguyen Van A",
    "orderDate": "2026-06-24T10:30:00",
    "orderStatus": "Đang xử lý",
    "paymentMethod": "Chuyển khoản",
    "paymentStatus": "Chưa thanh toán",
    "totalOriginalAmount": 56000,
    "discountAmount": 0,
    "shippingFee": 0,
    "totalPayAmount": 56000,
    "receiverName": "Nguyen Van A",
    "receiverPhone": "0901234567",
    "shippingAddress": "123 Le Loi, Quan 1, TP.HCM",
    "note": "Giao buổi sáng",
    "items": [
      {
        "orderItemId": 1,
        "productId": 10,
        "productName": "Sữa tươi",
        "price": 28000,
        "quantity": 2,
        "amount": 56000
      }
    ]
  },
  "errorMessage": null,
  "statusCode": 200
}
```

Case khong tim thay `404`: errorMessage `Không tìm thấy đơn hàng.`

FE action:

- Hien status action theo valid transition.
- Hien payment action doc lap voi order status.

### 8.30 `PUT /api/admin/orders/{id}/status?statusValue={value}`

Muc dich: doi trang thai don hang.

Auth: Admin.

Input:

```http
PUT /api/admin/orders/1/status?statusValue=3
```

Valid transition:

- 1 -> 2 hoac 5
- 2 -> 3 hoac 5
- 3 -> 4
- 4 -> khong doi
- 5 -> khong doi

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy đơn hàng để cập nhật trạng thái.`

Case transition khong hop le `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Không thể chuyển trạng thái đơn hàng từ 'Đang xử lý' sang 'Đã nhận hàng'.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

Case don da delivered/cancelled `400`:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Đơn hàng đã hủy, không thể thay đổi trạng thái.",
  "detail": "Lỗi xử lý nghiệp vụ hệ thống."
}
```

FE action:

- Chi hien option hop le de tranh loi.
- Neu chuyen sang huy, thong bao se tra lai ton kho.
- Sau thanh cong fetch lai order detail va list.

### 8.31 `PUT /api/admin/orders/{id}/payment-status?paymentStatusValue={value}`

Muc dich: doi trang thai thanh toan.

Auth: Admin.

Input:

```http
PUT /api/admin/orders/1/payment-status?paymentStatusValue=2
```

Rules:

- `paymentStatusValue = 1`: Chưa thanh toán.
- `paymentStatusValue = 2`: Đã thanh toán.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy đơn hàng để cập nhật trạng thái thanh toán.`

Case value sai `400`: middleware tra message possible values.

FE action:

- Sau cap nhat refetch detail/list.
- Nen confirm khi chuyen tu paid ve unpaid neu business yeu cau.

### 8.32 `GET /api/admin/customers`

Muc dich: lay danh sach customer admin.

Auth: Admin.

Input query:

```http
GET /api/admin/customers?searchTerm=0901&isActive=true&pageNumber=1&pageSize=10
```

Rules:

- `searchTerm`: search phone, full name, email.
- `isActive`: optional boolean.
- `pageNumber`, `pageSize`: paging.

Case thanh cong `200`: `ApiResponse<PagedResult<AdminCustomerDto>>`.

FE action:

- `totalSpent` khong tinh order da huy.
- Row click sang customer detail.

### 8.33 `GET /api/admin/customers/{id}`

Muc dich: lay chi tiet customer.

Auth: Admin.

Input path: `customerId`.

Case thanh cong `200`: `ApiResponse<AdminCustomerDetailDto>`.

Case khong tim thay `404`: errorMessage `Không tìm thấy khách hàng.`

FE action:

- Dia chi hien tai `fullAddress` chi map `SpecificAddress`, chua ghep wards/district/province. Neu can day du, backend can sua DTO.

### 8.34 `PUT /api/admin/customers/{id}/lock`

Muc dich: khoa customer.

Auth: Admin.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy khách hàng để khóa.`

FE action:

- Confirm truoc khi khoa.
- Sau khi khoa, customer khong login duoc vi login filter `IsActive`.

### 8.35 `PUT /api/admin/customers/{id}/unlock`

Muc dich: mo khoa customer.

Auth: Admin.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy khách hàng để mở khóa.`

### 8.36 `GET /api/admin/staffs`

Muc dich: lay danh sach nhan vien.

Auth: Admin.

Input: none.

Case thanh cong `200`:

```json
{
  "isSuccess": true,
  "data": [
    {
      "staffId": 1,
      "fullName": "Admin",
      "email": "admin@vncmart.local",
      "isActive": true,
      "createdAt": "2026-06-24T10:00:00",
      "roleId": 1,
      "roleName": "Admin"
    }
  ],
  "errorMessage": null,
  "statusCode": 200
}
```

FE action:

- Hien lock/unlock theo `isActive`.
- Chua co create/edit staff.

### 8.37 `GET /api/admin/staffs/{id}`

Muc dich: lay chi tiet nhan vien.

Auth: Admin.

Input path: `staffId`.

Case thanh cong `200`: `ApiResponse<AdminStaffDto>`.

Case khong tim thay `404`: errorMessage `Không tìm thấy nhân viên.`

### 8.38 `PUT /api/admin/staffs/{id}/lock`

Muc dich: khoa nhan vien.

Auth: Admin.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy nhân viên để khóa.`

FE action:

- Nen chan admin tu khoa chinh minh o UI neu backend chua co guard.

### 8.39 `PUT /api/admin/staffs/{id}/unlock`

Muc dich: mo khoa nhan vien.

Auth: Admin.

Case thanh cong `200`: `ApiResponse<boolean>` voi `data = true`.

Case khong tim thay `404`: errorMessage `Không tìm thấy nhân viên để mở khóa.`

## 9. Route map front-end de xuat

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

## 10. Trang va chuc nang can xay dung

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

## 11. State management de xuat

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

## 12. API client de xuat

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

## 13. UX/UI de xuat

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

## 14. Route guard

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

## 15. Nhung gioi han backend hien tai FE can biet

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

## 16. Checklist trien khai front-end

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

## 17. De xuat cai thien backend truoc khi FE hoan thien

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
