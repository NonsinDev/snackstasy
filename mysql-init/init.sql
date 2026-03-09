CREATE TABLE IF NOT EXISTS users (
  user_id INT AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(255) NOT NULL,
  last_name VARCHAR(255) NOT NULL,
  birthday DATE NULL,
  balance DECIMAL(10, 2) NOT NULL DEFAULT 0.00
);

CREATE TABLE IF NOT EXISTS employees (
  employee_id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(255) NOT NULL UNIQUE,
  first_name VARCHAR(255) NOT NULL,
  last_name VARCHAR(255) NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  role ENUM('admin', 'manager', 'cashier', 'staff') NOT NULL DEFAULT 'staff',
  stand_id INT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS stands (
  stand_id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  pickup_id VARCHAR(255) NOT NULL,
  tablet_id VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS items (
  item_id INT AUTO_INCREMENT PRIMARY KEY,
  stand_id INT NOT NULL,
  name VARCHAR(255) NOT NULL,
  price DECIMAL(10, 2) NOT NULL,
  stock INT NOT NULL,
  FOREIGN KEY (stand_id) REFERENCES stands(stand_id)
);

-- Default admin user (password: admin123)
INSERT INTO employees (username, password_hash, first_name, last_name, role) 
VALUES ('admin', '$2a$11$rBNr6wCBWVH8vPQzMNpJuO3Xf4NzIK4vYHGqZxVmZPwXvxZjZjZjZ', 'Admin', 'User', 'admin')
ON DUPLICATE KEY UPDATE first_name = first_name;
