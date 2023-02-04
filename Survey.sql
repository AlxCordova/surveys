USE [Acme]
GO
/****** Object:  Table [dbo].[input_types]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[input_types](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[survey_questions]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[survey_questions](
	[id] [int] NOT NULL,
	[survey_id] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[title] [varchar](250) NULL,
	[required] [varchar](1) NULL,
	[order] [int] NULL,
	[input_type_id] [int] NULL,
 CONSTRAINT [pk_survey_questions] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[survey_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[survey_responses]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[survey_responses](
	[survey_id] [int] NOT NULL,
	[survey_question_id] [int] NOT NULL,
	[response] [varchar](max) NULL,
	[created_at] [datetime] NULL,
 CONSTRAINT [pk_survey_responses] PRIMARY KEY CLUSTERED 
(
	[survey_id] ASC,
	[survey_question_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[surveys]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[surveys](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[description] [varchar](250) NULL,
	[link] [varchar](250) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](50) NOT NULL,
	[last_name] [varchar](50) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[password] [varchar](250) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[survey_responses] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[surveys] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[survey_questions]  WITH CHECK ADD FOREIGN KEY([input_type_id])
REFERENCES [dbo].[input_types] ([id])
GO
ALTER TABLE [dbo].[survey_questions]  WITH CHECK ADD FOREIGN KEY([survey_id])
REFERENCES [dbo].[surveys] ([id])
GO
ALTER TABLE [dbo].[survey_responses]  WITH CHECK ADD FOREIGN KEY([survey_id])
REFERENCES [dbo].[surveys] ([id])
GO
ALTER TABLE [dbo].[survey_responses]  WITH CHECK ADD FOREIGN KEY([survey_question_id], [survey_id])
REFERENCES [dbo].[survey_questions] ([id], [survey_id])
GO
/****** Object:  StoredProcedure [dbo].[GetErrorMessage]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetErrorMessage]
AS
BEGIN
declare @id int = 500

select null [data], @id [status], ERROR_MESSAGE() [message]

END
GO
/****** Object:  StoredProcedure [dbo].[sp_InputTypes]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InputTypes]  
    @action varchar(1),  
 @id int = null,  
    @name varchar(50) = null  
AS  
BEGIN TRY  
  
-- create new input type  
if(@action = 'C')  
begin  
 insert into input_types ([name])  
 values (@name)  
  
 SELECT SCOPE_IDENTITY()  
end  
  
-- get all types  
if(@action = 'R')  
begin  
 select *   
 from input_types 
 where id = ISNULL(@id, id)
end  
  
-- delete input type  
if(@action = 'D')  
begin  
 delete from input_types  
 where id = @id  
end  
  
END TRY  
BEGIN CATCH  
 exec GetErrorMessage  
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[sp_SurveyQuestions]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SurveyQuestions]
    @action varchar(1),
    @id int = null,
	@survey_id int = null,
	@name varchar(50) = null,
	@title varchar(250) = null,
	@required varchar(1) = null,
	@order int = null,
	@input_type_id int = null
AS
BEGIN TRY

-- create new survey question
if(@action = 'C')
begin

	declare @question_id int = 0
	set @question_id = (select ISNULL(MAX(id),0) + 1 from survey_questions where survey_id=@survey_id)

	insert into survey_questions (id, survey_id, [name], title, [required], [order], input_type_id)
	values (@question_id, @survey_id, @name, @title, @required, @order, @input_type_id)

	select SCOPE_IDENTITY()
end

-- get survey questions by survey_id 
if(@action = 'R')
begin
	select q.id, q.name, q.title, q.required, q.[order], t.name input_type, t.id input_type_id
	from survey_questions q
	inner join input_types t on 
	t.id = q.input_type_id
	where q.survey_id=@survey_id
	order by q.[order]
end

-- update survey question
if(@action = 'U')
begin
	update survey_questions set [name] = isnull(@name, [name]),
	title = isnull(@title, title), 
	[required] = isnull(@required, [required]),
	[order] = isnull(@order, [order]),
	input_type_id = isnull(@input_type_id, input_type_id)
	where survey_id=@survey_id and id=@id
end

-- delete survey question by id
if(@action = 'D')
begin
	delete from survey_questions
	where survey_id=@survey_id and id=@id
end

END TRY
BEGIN CATCH
	exec GetErrorMessage
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[sp_SurveyResponses]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SurveyResponses]
    @action varchar(1),
	@survey_id int = null,
	@survey_question_id int = null,
	@response varchar(max) = null
AS
BEGIN TRY

-- create new survey_response
if(@action = 'C')
begin
	insert into survey_responses (survey_id, survey_question_id, response)
	values (@survey_id, @survey_question_id, @response)
end

-- get survey_responses
if(@action = 'R')
begin
	select q.id question_id, q.[name], q.title, q.[required], q.[order], r.response, r.created_at
	from survey_responses r
	inner join survey_questions q on 
	q.survey_id=r.survey_id and
	q.id=r.survey_question_id 
	where r.survey_id=@survey_id 
	and survey_question_id=ISNULL(@survey_question_id, survey_question_id)
	order by q.[order]
end

END TRY
BEGIN CATCH
	exec GetErrorMessage
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[sp_Surveys]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Surveys]  
	@action varchar(1),  
	@id int = null,  
	@name varchar(100) = null,  
	@description varchar(250) = null,  
	@link varchar(250) = null  
AS  
BEGIN TRY  
  
-- create new survey  
if(@action = 'C')  
begin  
 insert into surveys ([name], [description], link)  
 values (@name, @description, @link)  
  
 select SCOPE_IDENTITY()  
end  
  
-- get survey by id or all active surveys  
if(@action = 'R')  
begin  
 select id survey_id, id, [name], [description], link, created_at  
 from surveys   
 where id = isnull(@id, id)  
 and deleted_at is null
end  
  
-- update survey  
if(@action = 'U')  
begin  
 update surveys set [name] = isnull(@name, [name]),  
 [description] = isnull(@description, [description]),   
 link = isnull(@link, link), updated_at=getdate()  
 where id = @id  
end  
  
-- delete survey / soft delete  
if(@action = 'D')  
begin  
 update surveys set deleted_at=getdate()  
 where id = @id  
end  

-- get survey by link  
if(@action = 'L')  
begin  
 select id, [name], [description], link, created_at  
 from surveys   
 where link = isnull(@link, link)  
end 
  
END TRY  
BEGIN CATCH  
 exec GetErrorMessage  
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[sp_Users]    Script Date: 2/3/2023 11:10:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Users]   
	@action varchar(1),  
	@first_name varchar(50) = null,  
	@last_name varchar(50) = null,  
	@email varchar(50) = null,  
	@password varchar(250) = null  
AS  
BEGIN TRY  
  
-- create new user  
if(@action = 'C')  
begin  
 insert into users (first_name, last_name, email, [password])  
 values (@first_name, @last_name, @email, @password)  
  
 SELECT SCOPE_IDENTITY()  
end  
  
-- check if user is valid, for login only  
if(@action = 'R')  
begin  
 select id, first_name, last_name, email, created_at, [password]  
 from users   
 where email = @email
 and deleted_at is null  
end  
  
-- update user  
if(@action = 'U')  
begin  
 update users set first_name = isnull(@first_name, first_name),  
 last_name = isnull(@last_name, last_name), [password] = isnull(@password, password),  
 updated_at=getdate()  
 where email = @email  
end  
  
-- delete user / soft delete  
if(@action = 'D')  
begin  
 update users set deleted_at=getdate()  
 where email = @email   
end  
  
END TRY  
BEGIN CATCH  
 exec GetErrorMessage  
END CATCH
GO
