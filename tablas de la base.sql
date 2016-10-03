CREATE TABLE PROYECTO(
	ID		CHAR(7),
	NOMBRE		VARCHAR(70),
	DESCRIPCION	VARCHAR(200),
	FECHAINICIO	DATE,
	FECHAFINAL	DATE,
	DURACION	INT,
	PRIMARY	KEY (ID)
);

CREATE TABLE USUARIOS(
	CEDULA		CHAR(10),
	NOMBRE		VARCHAR(20),
	PRYCTOID	CHAR(7),
	LIDER		BIT,
	FOREIGN KEY (PRYCTOID) REFERENCES PROYECTO(ID),
	PRIMARY KEY(CEDULA)	
);

CREATE TABLE TELEFONOS(
	CEDULA	CHAR(10),
	NUMERO	CHAR(10),
	FOREIGN KEY (CEDULA) REFERENCES USUARIOS(CEDULA),
	PRIMARY KEY (CEDULA, NUMERO)
)

CREATE TABLE CORREOS(
	CEDULA	CHAR(10),
	CORREO	VARCHAR(70),
	FOREIGN KEY (CEDULA) REFERENCES USUARIOS(CEDULA),
	PRIMARY KEY (CEDULA, CORREO)
)

CREATE TABLE REQUERIMIENTOS(
	ID			CHAR(7),
	NOMBRE			VARCHAR(70),
	ESFUERZO		INT,
	IMAGEN			IMAGE,
	DESCRIPCION		VARCHAR(200),
	PRIORIDAD		INT,
	OBSERVACIONES	VARCHAR(700),
	MODULO			VARCHAR(70),
	FECHAINCIO		DATE,
	FECHAFINAL		DATE,
	-- A = asignado P = pendiente E = en ejecucion F = finalizado C = cerrado
	ESTADO			CHAR(1),
	ENCARGADO		CHAR(10),
	PRYCTOID		CHAR(7),
	VERSION_ID		INT,
	FOREIGN KEY	(ENCARGADO) REFERENCES USUARIOS(CEDULA),
	FOREIGN KEY (PRYCTOID) REFERENCES	PROYECTO(ID),
	PRIMARY KEY (ID, VERSION_ID)
);

CREATE TABLE CAMBIOS(
	ID			CHAR(7),
	CEDULA			CHAR(10),
	FECHA			DATE,
	DESCRIPCION		VARCHAR(200),
	JUSTIFICACION		VARCHAR(700),
	REQUERIMIENTO_ID   	CHAR(7),
	VERSION_ID		INT,
	PRIMARY KEY (ID),
	FOREIGN KEY (REQUERIMIENTO_ID, VERSION_ID) REFERENCES REQUERIMIENTOS(ID, VERSION_ID),
	FOREIGN KEY (CEDULA) REFERENCES USUARIOS(CEDULA)
);

CREATE TABLE CRIT_ACEPTACION(
	ID			CHAR(7),
	DESCRIPCION		VARCHAR(200),
	REQUERIMIENTO_ID   	CHAR(7),
	VERSION_ID		INT,
	PRIMARY KEY (ID),
	FOREIGN KEY (REQUERIMIENTO_ID, VERSION_ID) REFERENCES REQUERIMIENTOS(ID, VERSION_ID)
);

CREATE TABLE PERMISOS(
	ID			NVARCHAR(128),
	NOMBRE 		VARCHAR(70),
	DESCRIPCION	VARCHAR(200),
	PRIMARY 	KEY(ID)
);

CREATE TABLE MANEJA(
	ID		NVARCHAR(128),
	FOREIGN KEY	(ID) REFERENCES PERMISOS(ID),
);
