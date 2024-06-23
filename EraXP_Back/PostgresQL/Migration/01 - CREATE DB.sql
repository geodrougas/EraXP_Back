CREATE TABLE photos (
    id uuid NOT NULL,
    name varchar(100) NOT NULL,
    uri varchar(2000) NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE users (
    id uuid NOT NULL,
    username varchar(50) NOT NULL,
    base64_hashed_password varchar(1024) NOT NULL,
    normalized_username VARCHAR(50) NOT NULL,
    email VARCHAR(150) NOT NULL,
    normalized_email VARCHAR(150) NOT NULL,
    user_type int NOT NULL,
    security_stamp uuid NOT NULL,
    concurrency_stamp uuid NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE universities (
    id uuid NOT NULL,
    name varchar(50) NOT NULL,
    description text NOT NULL,
    thumbnail_id uuid NULL,
    university_language text NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (thumbnail_id) REFERENCES photos(id) ON DELETE CASCADE
);

CREATE TABLE addresses (
    id uuid NOT NULL,
    university_id uuid NOT NULL,
    name varchar(50) NOT NULL,
    google_location_id varchar(150) NOT NULL,
    latitude numeric(13, 9) NOT NULL,
    longitude numeric(13, 9) NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (university_id) REFERENCES universities(id) ON DELETE CASCADE
);

CREATE TABLE university_photos (
    id uuid NOT NULL,
    university_id uuid NOT NULL,
    photo_id uuid NULL,
    uri varchar(2035) NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (university_id) REFERENCES universities(id) ON DELETE CASCADE,
    FOREIGN KEY (photo_id) REFERENCES photos(id) ON DELETE CASCADE
);

CREATE TABLE departments (
    id uuid NOT NULL,
    university_id uuid NOT NULL,
    name varchar(50) NOT NULL,
    description text NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (university_id) REFERENCES universities(id) ON DELETE CASCADE
);

CREATE TABLE courses (
    id uuid NOT NULL,
    department_id uuid NOT NULL,
    semester varchar(500) NOT NULL,
    name varchar(50) NOT NULL,
    description text NOT NULL,
    ects numeric(18, 8) NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE CASCADE
);

CREATE TABLE contacts (
    id uuid NOT NULL,
    department_id uuid NOT NULL,
    name varchar(50) NOT NULL,
    lastname varchar(50) NOT NULL,
    email varchar(150) NOT NULL,
    phone_number varchar(20) NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE CASCADE
);

create table student_university_info (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    university_id uuid NOT NULL,
    department_id uuid NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (university_id) REFERENCES universities(id) ON DELETE CASCADE,
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE CASCADE
);

create table professor_university_info (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    university_id uuid NOT NULL,
    FOREIGN KEY (university_id) REFERENCES universities(id) ON DELETE CASCADE
);

insert into users 
    (id, username, base64_hashed_password, normalized_username, email, normalized_email, user_type, security_stamp, concurrency_stamp) 
values
    (
     '077eb2be-9484-47e5-a1df-7003acc91261',
     'admin',
     'KwEWy36Lb4ktcHo2ID4GoUUW9CLRjYVE7GKtFDVbBj3EEqIipAHwCkIw9HRAu3SmyOSPQRgkd/ycK+EBdIw/Kg==',
     'ADMIN',
     'geodrougas@gmail.com',
     'GEODROUGAS@GMAIL.COM',
     '999',
     '20a68084-0de4-4265-aa16-8e79c2c21e3f',
     '67956e64-ad54-4756-ad18-a08a1cbfb425'
    )