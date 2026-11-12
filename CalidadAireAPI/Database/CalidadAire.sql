CREATE DATABASE IF NOT EXISTS calidad_aire;

USE calidad_aire;

CREATE TABLE sensores_calidad_aire (
    id INT AUTO_INCREMENT PRIMARY KEY,
    ubicacion VARCHAR(150) NOT NULL,
    tipo_gas VARCHAR(80) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Activo',

    CONSTRAINT chk_sensor_estado
        CHECK (estado IN ('Activo', 'Inactivo', 'Mantenimiento'))
) ENGINE = InnoDB;

CREATE TABLE lecturas_aire (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sensor_id INT NOT NULL,
    pm2_5 DECIMAL(10, 2) NOT NULL,
    pm10 DECIMAL(10, 2) NOT NULL,
    co2 DECIMAL(10, 2) NOT NULL,
    fecha_hora DATETIME(6) NOT NULL,

    CONSTRAINT fk_lectura_sensor
        FOREIGN KEY (sensor_id)
        REFERENCES sensores_calidad_aire(id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_lectura_pm25
        CHECK (pm2_5 >= 0),

    CONSTRAINT chk_lectura_pm10
        CHECK (pm10 >= 0),

    CONSTRAINT chk_lectura_co2
        CHECK (co2 >= 0)

) ENGINE = InnoDB;

CREATE TABLE alertas_aire (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sensor_id INT NOT NULL,
    nivel VARCHAR(20) NOT NULL,
    mensaje VARCHAR(500) NOT NULL,
    fecha_hora DATETIME(6) NOT NULL,

    CONSTRAINT fk_alerta_sensor
        FOREIGN KEY (sensor_id)
        REFERENCES sensores_calidad_aire(id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_alerta_nivel
        CHECK (nivel IN ('Leve', 'Moderada', 'Critica', 'Extrema'))

) ENGINE = InnoDB;

CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL UNIQUE,
    contrasena_hash VARCHAR(255) NOT NULL
);

INSERT INTO sensores_calidad_aire (ubicacion, tipo_gas, estado)
VALUES 	('San Salvador, El Salvador','CO2','Activo'),
('Santa Tecla, El Salvador','CO2','Activo'),
('Santa Ana, El Salvador','CO2','Activo');