CREATE TABLE IF NOT EXISTS users (
  user_id INT AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(255) NOT NULL,
  last_name VARCHAR(255) NOT NULL,
  birthday DATE NULL,
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
  description TEXT NULL,
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

CREATE TABLE IF NOT EXISTS orders (
  order_id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  stand_id INT NOT NULL,
  total_price DECIMAL(10, 2) NOT NULL,
  status ENUM('pending', 'preparing', 'ready', 'completed', 'cancelled') DEFAULT 'pending',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (user_id) REFERENCES users(user_id),
  FOREIGN KEY (stand_id) REFERENCES stands(stand_id)
);

CREATE TABLE IF NOT EXISTS order_items (
  order_item_id INT AUTO_INCREMENT PRIMARY KEY,
  order_id INT NOT NULL,
  item_id INT NOT NULL,
  is_collected BOOLEAN DEFAULT FALSE,
  quantity INT NOT NULL,
  price DECIMAL(10, 2) NOT NULL,
  FOREIGN KEY (order_id) REFERENCES orders(order_id),
  FOREIGN KEY (item_id) REFERENCES items(item_id)
);

-- Admin Password: admin1234
-- Staff Password: staff1234
 INSERT INTO employees (username, password_hash, first_name, last_name, role, stand_id)
 VALUES 
    ('admin', '$2a$11$y6j8z.rUt29cp/LJ/yko/eG832ErL17.t/PYgAngitthGndkto2Ra', 'Admin', 'User', 'admin', 1),
    ('staff01', '$2a$11$s.s.w9E.XL50yuz108raZ.94XqZf4AUKMmtqvvoSq0PxjAL2oqtru', 'Staff', '01', 'staff', 2)
 ON DUPLICATE KEY UPDATE first_name = first_name;

-- Users
INSERT INTO users (user_id, first_name, last_name, birthday, balance, ticket_id)
VALUES
  (1,'Max', 'Rubel', '1990-01-01', 100.00, 'MR123456'),
  (2,'Silas', 'Mohr', '1992-05-15', 50.00, 'SM654321'),
  (3,'Daniel', 'Jung', '1985-10-20', 75.00, 'JD789012')
ON DUPLICATE KEY UPDATE ticket_id = ticket_id;

INSERT INTO stands (stand_id, name, description, pickup_id, tablet_id)
VALUES
  (1,'Burger House', 'Delicious burgers and fries', 1, 1),
  (2,'Pizza World', 'Authentic Italian pizza', 2, 2),
  (3,'Asia Wok', 'Fresh Asian cuisine', 3, 3),
  (4,'Corndog Home', 'Tasty corndogs and snacks', 4, 4),
  (5,'Sushi Palace', 'Fresh sushi and sashimi', 5, 5),
  (6,'Tacco Island', 'Delicious tacos and burritos', 6, 6)
ON DUPLICATE KEY UPDATE stand_id = stand_id;

-- Default items
INSERT INTO items (item_id,stand_id, name, price, stock)
VALUES
  (1, 1, 'Burger Classic', 5.99, 20),
  (2, 1, 'Cheeseburger', 6.49, 18),
  (3, 1, 'Double Cheeseburger', 7.99, 12),
  (4, 1, 'Chicken Burger', 6.99, 15),
  (5, 1, 'Veggie Burger', 6.49, 10),
  (6, 1, 'Bacon Burger', 7.49, 14),
  (7, 1, 'BBQ Burger', 7.99, 11),
  (8 ,1, 'Spicy Burger', 6.99, 13),
  (9 ,1, 'Mini Burger', 4.99, 25),
  (10, 1, 'Kids Burger', 4.49, 30),

  (11, 1, 'Fries Small', 2.49, 40),
  (12, 1, 'Fries Large', 3.49, 35),
  (13, 1, 'Cheese Fries', 3.99, 20),
  (14, 1, 'Chili Fries', 4.49, 18),
  (15, 1, 'Sweet Potato Fries', 4.99, 16),

  (16, 1, 'Chicken Nuggets', 5.49, 22),
  (17, 1, 'Hot Wings', 6.99, 15),
  (18, 1, 'Onion Rings', 3.99, 19),
  (19, 1, 'Mozzarella Sticks', 4.99, 17),
  (20, 1, 'Garlic Bread', 3.49, 21),

  (21, 1, 'Cola 0.5L', 2.49, 50),
  (22, 1, 'Fanta 0.5L', 2.49, 45),
  (23, 1, 'Sprite 0.5L', 2.49, 45),
  (24, 1, 'Wasser 0.5L', 1.99, 60),
  (25, 1, 'Eistee 0.5L', 2.79, 40),

  (26, 2, 'Cola 0.5L', 2.49, 50),
  (27, 2, 'Fanta 0.5L', 2.49, 45),
  (28, 2, 'Sprite 0.5L', 2.49, 45),
  (29, 2, 'Wasser 0.5L', 1.99, 60),
  (30, 2, 'Eistee 0.5L', 2.79, 40),

  (31, 3, 'Cola 0.5L', 2.49, 50),
  (32, 3, 'Fanta 0.5L', 2.49, 45),
  (33, 3, 'Sprite 0.5L', 2.49, 45),
  (34, 3, 'Wasser 0.5L', 1.99, 60),
  (35, 3, 'Eistee 0.5L', 2.79, 40)
ON DUPLICATE KEY UPDATE item_id = item_id;