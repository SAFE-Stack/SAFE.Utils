open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO
open Fake.JavaScript

type Project =
    | Server
    | Client

    static member fromString input =
        match input with
        | "client" -> Client
        | "server" -> Server
        | other -> failwith $"unknown project {other}"

    member this.Name =
        match this with
        | Server -> "Server"
        | Client -> "Client"

    member this.sourceFolder = $"../src/{this.Name}"

let execContext = Context.FakeExecutionContext.Create false "build.fsx" []
Context.setExecutionContext (Context.RuntimeContext.Fake execContext)


module Environment =

    let projectAndVersion () =
        let versionComponents = (Environment.environVarOrFail "PROJECT_VERSION").Split '_'

        let project = versionComponents[0] |> Project.fromString
        let version = versionComponents[1]

        project, version

    let releaseNotes () =
        Environment.environVarOrFail "RELEASE_NOTES_URL"


module Processes =
    let createProcess exe args dir =
        // Use `fromRawCommand` rather than `fromRawCommandLine`, as its behaviour is less likely to be misunderstood.
        // See https://github.com/SAFE-Stack/SAFE-template/issues/551.
        CreateProcess.fromRawCommand exe args
        |> CreateProcess.withWorkingDirectory dir
        |> CreateProcess.ensureExitCode


    let run proc arg dir = proc arg dir |> Proc.run |> ignore

    let dotnet = createProcess "dotnet"

    let runDotnet = run dotnet

let outputFolder = Path.getFullName """../nugetPackages"""

let clientTestFolder = Path.getFullName """../test/Client"""


Target.create "Test" (fun _ ->
    Npm.install (fun o ->
        { o with
            WorkingDirectory = clientTestFolder })

    Npm.run "test" (fun o ->
        { o with
            WorkingDirectory = clientTestFolder }))

Target.create "Bundle" (fun _ ->

    let releaseNotes = Environment.releaseNotes

    let project, version = Environment.projectAndVersion ()

    Processes.runDotnet
        [ "pack"
          "-o"
          outputFolder
          $"-p:PackageVersion={version}"
          $"-p:PackageReleaseNotes={releaseNotes}" ]
        project.sourceFolder)


Target.create "Publish" (fun _ ->
    let nugetApiKey = Environment.environVarOrFail "NUGET_API_KEY"

    let nugetArgs =
        [ "push"
          "*.nupkg"
          "--api-key"
          nugetApiKey
          "--source"
          """https://api.nuget.org/v3/index.json""" ]

    Processes.runDotnet [ "nuget"; yield! nugetArgs ] outputFolder)

"Bundle" ==> "Publish" |> ignore

[<EntryPoint>]
let main args =
    try
        match args with
        | [| target |] -> Target.runOrDefault target
        | _ -> Target.runOrDefault "Bundle"

        0
    with e ->
        printfn "%A" e
        1
