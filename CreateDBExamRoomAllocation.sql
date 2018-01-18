CREATE TABLE Stream (
  Id INT ,
  Name NVARCHAR(255) ,
  CONSTRAINT stream_id_pk PRIMARY KEY (Id)
 
);

CREATE TABLE Department (
   Id INT,
   Name NVARCHAR(255) ,
   Stream_Id INT ,
   CONSTRAINT department_id_pk PRIMARY KEY (Id) ,
   CONSTRAINT dept_str_id_fk FOREIGN KEY(Stream_Id) REFERENCES Stream(Id)
   ON DELETE CASCADE
   ON UPDATE CASCADE 
);

CREATE TABLE Designation (
   Id INT ,
   Name NVARCHAR(255) ,
   CONSTRAINT designation_id_pk PRIMARY KEY (Id)
 
);

CREATE TABLE Teacher (
   Id NVARCHAR(255)  ,
   Name NVARCHAR(255) ,
   Experience INT ,
   Designation_Id INT ,
   Department_Id INT ,
   CONSTRAINT teacher_id_pk PRIMARY KEY (Id) ,
   CONSTRAINT teacher_dept_id_fk FOREIGN KEY(Department_Id) REFERENCES     Department(Id) ,
   CONSTRAINT teacher_desg_id_fk FOREIGN KEY(Designation_Id) REFERENCES Designation(Id)
   ON DELETE CASCADE
   ON UPDATE CASCADE 
);



CREATE TABLE Room (
   Id INT  ,
   No INT ,
   Block NVARCHAR(255) ,
   Capacity INT ,
   CONSTRAINT room_id_pk PRIMARY KEY (Id)

);

CREATE TABLE Student (
   USN NVARCHAR(255) ,
   Name NVARCHAR(255) ,
   Sem INT ,
   Id INT ,
   CONSTRAINT student_usn_pk PRIMARY KEY (USN) ,
   CONSTRAINT student_id_fk FOREIGN KEY(Id) REFERENCES Department(Id)
   ON DELETE CASCADE
   ON UPDATE CASCADE 
);

CREATE TABLE Exam (
   Code NVARCHAR(255) ,
   Type NVARCHAR(255) ,
   Name NVARCHAR(255) ,
   Date DATE ,
   Id INT ,
   CONSTRAINT exam_code_pk PRIMARY KEY (Code) ,
   CONSTRAINT exam_id_fk FOREIGN KEY(Id) REFERENCES Department(Id)
   ON DELETE CASCADE
   ON UPDATE CASCADE 
);


CREATE TABLE Session (
   Id INT ,
   Type NVARCHAR(255) ,
   CONSTRAINT session_id_pk PRIMARY KEY (Id)

);

CREATE TABLE StudentWriteExam (
  USN NVARCHAR(255) NOT NULL REFERENCES Student(USN),
  Code NVARCHAR(255) NOT NULL REFERENCES Exam(Code),
  CONSTRAINT swe_composite_pk PRIMARY KEY (USN , Code));


CREATE TABLE ExamInRoom (
  Code NVARCHAR(255) NOT NULL REFERENCES Exam(Code),
  Id INT NOT NULL REFERENCES Room(Id),
  CONSTRAINT eir_composite_pk PRIMARY KEY (Code,Id));


CREATE TABLE TeacherSupervisesExam (
   Id NVARCHAR(255) NOT NULL REFERENCES Teacher(Id),
   Code NVARCHAR(255) NOT NULL REFERENCES Exam(Code),
CONSTRAINT tse_composite_pk PRIMARY KEY (Id , Code));