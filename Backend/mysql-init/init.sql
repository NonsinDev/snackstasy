CREATE TABLE IF NOT EXISTS users (
  user_id INT AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(255) NOT NULL,
  last_name VARCHAR(255) NOT NULL,
  balance DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  ticket_id VARCHAR(255) NOT NULL UNIQUE
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
  pickup_id INT NOT NULL,
  tablet_id INT NOT NULL
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

-- Users (nur einfügen, wenn ticket_id noch nicht existiert)
INSERT INTO users (user_id, first_name, last_name, birthday, balance, ticket_id)
VALUES
  (1,'Max', 'Rubel', '1990-01-01', 100.00, 'MR123456'),
  (2,'Silas', 'Mohr', '1992-05-15', 50.00, 'SM654321'),
  (3,'Daniel', 'Jung', '1985-10-20', 75.00, 'JD789012')
ON DUPLICATE KEY UPDATE ticket_id = ticket_id;