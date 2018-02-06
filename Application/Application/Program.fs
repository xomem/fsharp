open Rezoom.SQL
open Rezoom.SQL.Migrations
open Rezoom.SQL.Synchronous // extension methods for running commands synchronously
// find comments by users whose names contain the given substring
type ListCommentsByUser = SQL<"""
    select u.Id, u.Email, c.Id as CommentId, c.Comment
    from Users u
    join Comments c on c.AuthorId = u.Id
    where u.Name like '%' || @name || '%'
""">

let showComments name =
    use context = new ConnectionContext()
    let comments = ListCommentsByUser.Command(name).Execute(context)
    printfn "There are %d comments by users matching the name `%s`." comments.Count name
    for comment in comments do
        printfn "User ID %d:" comment.Id
        printfn " Email: %s" comment.Email
        printfn " Comment ID: %d" comment.CommentId
        printfn " Comment: %s" comment.Comment


// define a SQL command for inserting a user and getting their ID
type InsertUser = SQL<"""
    insert into Users(Name, Email) values (@name, @email);
    select last_insert_rowid() as id
""">

// define a SQL command for inserting a comment 
type InsertComment = SQL<"insert into Comments(AuthorId, Comment) values (@authorId, @comment)">

let addExampleUser name email =
    // open a context in which to run queries
    use context = new ConnectionContext()
    // insert a user and get their ID
    let userId : int64 =
        InsertUser.Command(email = email, name = Some name).ExecuteScalar(context)
    // add a couple comments by the user
    InsertComment.Command(authorId = int userId, comment = "Comment 1").Execute(context)
    InsertComment.Command(authorId = int userId, comment = "Comment 2").Execute(context)

// edit your main function to call the new insert code
type MyModel = SQLModel<"."> // find migrations in the project folder, "."

type ListUsers = SQL<"""
    select * from Users
""">

let showUsers() =
    use context = new ConnectionContext()
    let users = ListUsers.Command().Execute(context)
    printfn "There are %d users." users.Count
    for user in users do
        printfn "User ID %d's email is %s..." user.Id user.Email
        match user.Name with
        | None -> printfn "  and they don't have a name."
        | Some name -> printfn "  and their name is %s." name

// edit your main function to call the new query code
let migrate() =
    // customize the default migration config so that it outputs a message after running a migration
    let config =
        { MigrationConfig.Default with
            LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName
        }
    // run the migrations, creating the database if it doesn't exist
    MyModel.Migrate(config)
[<EntryPoint>]
let main argv =
    migrate()
    showComments "Test"
    System.Console.ReadKey()
    // return 0 status code
    0