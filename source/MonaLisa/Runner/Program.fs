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
            Point(float fields.[1],float fields.[2]))
        |> Seq.toArray
    points
    
let file = ("..\\..\\..\\burma14.tsp", 14, 8)
//let file = ("..\\..\\..\\mona-lisa100K.tsp", 100000, 6)

[<EntryPoint>]
let main argv = 
    argv |> ignore
    let (filename, size, linesToSkip) = file
    let original = readFile filename size linesToSkip

    let rng = Random ()

    printfn "Number of points read: %i" original.Length
    let l = length original

    let sol1 = shuffle rng original
    let len1 = length sol1

    let sol2 = shuffle rng original
    let len2 = length sol2

    let sol3 = shuffle rng original
    let len3 = length sol3

    let sol4 = shuffle rng original
    let len4 = length sol4

    let sol5 = shuffle rng original
    let len5 = length sol5

    let sol6 = shuffle rng original
    let len6 = length sol6

    let sol7 = shuffle rng original
    let len7 = length sol7

    let sol8 = shuffle rng original
    let len8 = length sol8

    let sol9 = shuffle rng original
    let len9 = length sol9

    let sol10 = shuffle rng original
    let len10 = length sol10

    let sol11 = shuffle rng original
    let len11 = length sol11

    let sol12 = shuffle rng original
    let len12 = length sol12

    let sol13 = shuffle rng original
    let len13 = length sol13

    let sol14 = shuffle rng original
    let len14 = length sol14

    let sol15 = shuffle rng original
    let len15 = length sol15


    printfn "Length original: %f" l
    printfn "Length solution 1: %f" len1
    printfn "Length solution 2: %f" len2
    printfn "Length solution 3: %f" len3
    printfn "Length solution 4: %f" len4
    printfn "Length solution 5: %f" len5
    printfn "Length solution 6: %f" len6
    printfn "Length solution 7: %f" len7
    printfn "Length solution 8: %f" len8
    printfn "Length solution 9: %f" len9
    printfn "Length solution 10: %f" len10
    printfn "Length solution 11: %f" len11
    printfn "Length solution 12: %f" len12
    printfn "Length solution 13: %f" len13
    printfn "Length solution 14: %f" len14
    printfn "Length solution 15: %f" len15
    Console.ReadKey() |> ignore

    0 // return an integer exit code
