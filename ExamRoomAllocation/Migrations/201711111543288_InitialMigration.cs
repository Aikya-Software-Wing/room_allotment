namespace ExamRoomAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                        Stream_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stream", t => t.Stream_Id, cascadeDelete: true)
                .Index(t => t.Stream_Id);
            
            CreateTable(
                "dbo.Exam",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 255),
                        Type = c.String(maxLength: 255),
                        Name = c.String(maxLength: 255),
                        Date = c.DateTime(storeType: "date"),
                        Id = c.Int(),
                    })
                .PrimaryKey(t => t.Code)
                .ForeignKey("dbo.Department", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        No = c.Int(),
                        Block = c.String(maxLength: 255),
                        Capacity = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        USN = c.String(nullable: false, maxLength: 255),
                        Name = c.String(maxLength: 255),
                        Sem = c.Int(),
                        Id = c.Int(),
                    })
                .PrimaryKey(t => t.USN)
                .ForeignKey("dbo.Department", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Teacher",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 255),
                        Name = c.String(maxLength: 255),
                        Experience = c.Int(),
                        Designation_Id = c.Int(),
                        Department_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Designation", t => t.Designation_Id, cascadeDelete: true)
                .ForeignKey("dbo.Department", t => t.Department_Id)
                .Index(t => t.Designation_Id)
                .Index(t => t.Department_Id);
            
            CreateTable(
                "dbo.Designation",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stream",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Session",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Type = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExamInRoom",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 255),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Code, t.Id })
                .ForeignKey("dbo.Exam", t => t.Code, cascadeDelete: true)
                .ForeignKey("dbo.Room", t => t.Id, cascadeDelete: true)
                .Index(t => t.Code)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.StudentWriteExam",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 255),
                        USN = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => new { t.Code, t.USN })
                .ForeignKey("dbo.Exam", t => t.Code)
                .ForeignKey("dbo.Student", t => t.USN)
                .Index(t => t.Code)
                .Index(t => t.USN);
            
            CreateTable(
                "dbo.TeacherSupervisesExam",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 255),
                        Id = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => new { t.Code, t.Id })
                .ForeignKey("dbo.Exam", t => t.Code, cascadeDelete: true)
                .ForeignKey("dbo.Teacher", t => t.Id, cascadeDelete: true)
                .Index(t => t.Code)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teacher", "Department_Id", "dbo.Department");
            DropForeignKey("dbo.Student", "Id", "dbo.Department");
            DropForeignKey("dbo.Department", "Stream_Id", "dbo.Stream");
            DropForeignKey("dbo.Exam", "Id", "dbo.Department");
            DropForeignKey("dbo.TeacherSupervisesExam", "Id", "dbo.Teacher");
            DropForeignKey("dbo.TeacherSupervisesExam", "Code", "dbo.Exam");
            DropForeignKey("dbo.Teacher", "Designation_Id", "dbo.Designation");
            DropForeignKey("dbo.StudentWriteExam", "USN", "dbo.Student");
            DropForeignKey("dbo.StudentWriteExam", "Code", "dbo.Exam");
            DropForeignKey("dbo.ExamInRoom", "Id", "dbo.Room");
            DropForeignKey("dbo.ExamInRoom", "Code", "dbo.Exam");
            DropIndex("dbo.TeacherSupervisesExam", new[] { "Id" });
            DropIndex("dbo.TeacherSupervisesExam", new[] { "Code" });
            DropIndex("dbo.StudentWriteExam", new[] { "USN" });
            DropIndex("dbo.StudentWriteExam", new[] { "Code" });
            DropIndex("dbo.ExamInRoom", new[] { "Id" });
            DropIndex("dbo.ExamInRoom", new[] { "Code" });
            DropIndex("dbo.Teacher", new[] { "Department_Id" });
            DropIndex("dbo.Teacher", new[] { "Designation_Id" });
            DropIndex("dbo.Student", new[] { "Id" });
            DropIndex("dbo.Exam", new[] { "Id" });
            DropIndex("dbo.Department", new[] { "Stream_Id" });
            DropTable("dbo.TeacherSupervisesExam");
            DropTable("dbo.StudentWriteExam");
            DropTable("dbo.ExamInRoom");
            DropTable("dbo.Session");
            DropTable("dbo.Stream");
            DropTable("dbo.Designation");
            DropTable("dbo.Teacher");
            DropTable("dbo.Student");
            DropTable("dbo.Room");
            DropTable("dbo.Exam");
            DropTable("dbo.Department");
        }
    }
}
