# Luong Dung Chung Ha Tang MongoDB

Tai lieu nay mo ta cach du an dang dung chung ha tang MongoDB giua cac service qua `Play.Common`.

## 1) Muc tieu

- Giam lap code phan ket noi MongoDB, repository generic, va DI registration.
- Moi service chi can quan tam Entity, DTO, Controller, business flow.
- Cau hinh DB/collection van tach rieng theo tung service.

## 2) Cau truc hien tai

- `Play.Common`
  - `Entities/IEntity.cs`
  - `Repositories/IRepository.cs`
  - `Repositories/MongoRepository.cs`
  - `Settings/MongoDbSettings.cs`
  - `Settings/ServiceSettings.cs`
  - `Extensions/ServiceCollectionExtensions.cs`
- `PLAY.CATALOG/src/Play.Catalog.Service`
  - `Entities/Item.cs` (implement `IEntity`)
  - `Controllers/ItemController.cs` (inject `IRepository<Item>`)
  - `Program.cs` (goi extension dung chung)

## 3) Luong khoi tao (Startup Flow)

Trong `Program.cs` cua moi service:

```csharp
builder.Services.AddMongo().AddMongoRepository<Item>("items");
```

Y nghia:

1. `AddMongo()`
   - Dang ky serializer cho `Guid` va `DateTimeOffset`.
   - Doc `MongoDBSettingJSON` -> tao `MongoDbSettings`.
   - Doc `ServiceSettingJSON` -> tao `ServiceSettings`.
   - Dang ky `IMongoClient`.
   - Dang ky `IMongoDatabase` bang `ServiceSettings.ServiceName`.

2. `AddMongoRepository<Item>("items")`
   - Dang ky `IRepository<Item>` su dung `MongoRepository<Item>`.
   - Gan entity `Item` vao collection `"items"`.

## 4) Luong request API (Runtime Flow)

Vi du voi Item API:

1. Request vao `ItemController`.
2. `ItemController` dung `IRepository<Item>`.
3. DI resolve ra `MongoRepository<Item>`.
4. `MongoRepository<Item>` thao tac voi collection `items` trong database da cau hinh.
5. Ket qua tra ve Controller -> DTO -> Response.

## 5) Cau hinh moi service

Moi service can khai bao trong `appsettings.json`:

```json
{
  "MongoDBSettingJSON": {
    "Host": "localhost",
    "Port": "27017"
  },
  "ServiceSettingJSON": {
    "ServiceName": "Catalog"
  }
}
```

Luu y:

- `ServiceName` la ten database MongoDB cua service do.
- Hai service co the dung cung Mongo host nhung database khac nhau.

## 6) Cach tao service Mongo moi

1. Tao project service moi.
2. Add reference sang `Play.Common`.
3. Tao Entity implement `IEntity`.
4. Cau hinh `MongoDBSettingJSON` va `ServiceSettingJSON`.
5. Trong `Program.cs` goi:
   - `AddMongo()`
   - `AddMongoRepository<YourEntity>("your_collection")`
6. Trong Controller inject `IRepository<YourEntity>`.

## 7) Gioi han va huong nang cap

Hien tai `Play.Common` chua ca abstraction + Mongo implementation.

Neu sau nay co service dung PostgreSQL, nen tach:

- `Play.Common` (abstractions trung lap: `IEntity`, `IRepository<T>`)
- `Play.Infrastructure.Mongo` (Mongo implementation)
- `Play.Infrastructure.Postgres` (Postgres implementation)

Khi do moi service chi reference infrastructure phu hop voi DB ma no dung.

