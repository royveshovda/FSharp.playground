open System
open System.IO
open Calculator



type TspFilePart =
    | NamePart of name:string
    | TypePart of fileType:string
    | CommentPart of comment:string
    | EdgeWeightTypePart of edgeWeightType:string
    | DimensionPart of dimension:int
    | PointsPart of points:Point[]

type TspFile = {
    Name: string;
    Type: string;
    Comment: string;
    EdgeWeightType: string;
    Dimension: int;
    Points: Point[]}

let readFile (filename:string) size linesToSkip =
    use fs = File.OpenText(filename)
    let text = fs.ReadToEnd()
    //TODO: Improve parsing here
    let points =
        text.Split ('\n')
        |> Seq.skip linesToSkip
        |> Seq.take size
        |> Seq.map (fun line -> line.TrimStart().TrimEnd())
        //|> Seq.map (fun line -> line.Replace('.', ','))
        |> Seq.map (fun line -> line.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " "))
        |> Seq.map (fun line ->
            let fields = line.Split(' ')
            Point(int fields.[1],int fields.[2]))
        |> Seq.toArray
    points
    
let file = ("..\\..\\..\\burma14.tsp", 14, 8)
//let file = ("..\\..\\..\\mona-lisa100K.tsp", 100000, 6)


let printSoultion (solution:Solution) =
    Array.iter (fun (p:Point) -> printfn "%i %i" p.X p.Y) (solution)

let rec mutate iterations solution =
    let rng = Random ()

    let mutateAndTestOne rng solution =        
        let alternative = clone solution |> shuffle rng
        let len = length alternative
        printfn "Length: %i" len
        best solution alternative

    let rec mutateMany iteration iterations rng solution = 
        printf "Test %i  :" iteration
        match iteration >= iterations with
        | true -> solution
        | false ->
            let next = mutateAndTestOne rng solution
            mutateMany (iteration+1) iterations rng next

    mutateMany 0 iterations rng solution

[<EntryPoint>]
let main argv = 
    argv |> ignore
    let (filename, size, linesToSkip) = file
    let original = readFile filename size linesToSkip

    let rng = Random ()

    printfn "Number of points read: %i" original.Length
    let l = length original
    printfn "Length original: %i" l

    let choice = mutate 10000 original

    let lenBest = length choice
    printfn "Length best solution 1: %i" lenBest
    printSoultion choice
    
    Console.ReadKey() |> ignore

    0 // return an integer exit code
