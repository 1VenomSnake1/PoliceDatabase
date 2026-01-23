// Файл для инициализации MongoDB
db = db.getSiblingDB('admin');

db.auth('admin', 'admin123');

db = db.getSiblingDB('PoliceDatabase');

// Создаем пользователя приложения
db.createUser({
    user: "police_app",
    pwd: "police_app_password",
    roles: [
        { role: "readWrite", db: "PoliceDatabase" },
        { role: "dbAdmin", db: "PoliceDatabase" }
    ]
});

print(" Пользователь police_app создан в PoliceDatabase");

// Создаем коллекции и индексы
db.createCollection("Users");
db.createCollection("Cases");
db.createCollection("Evidences");
db.createCollection("PendingChanges");

print(" Коллекции созданы");