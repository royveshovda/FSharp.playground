module Calculator

open System

type Point =
        struct
            val X:float
            val Y:float
            new (x:float,y:float) = { X=x; Y=y }
        end

type Solution = Point array

let dist (p1:Point) (p2:Point) =
    let x1 = float p1.X
    let x2 = float p2.X
    let y1 = float p1.Y
    let y2 = float p2.Y
    Math.Sqrt( Math.Pow((x2-x1), 2.) + Math.Pow((y2-y1), 2.))



let length (xs:Solution) =    
    let len = xs |> Array.length
    xs

    |> Seq.fold (fun (acc,prev) x ->
        acc + dist x  xs.[prev], (prev+1)%len) (0.,len-1)
    |> fst

let length2 (xArray:Solution) =  

    let rec lengthRec (xs':Point list) acc first =
        match xs' with
        | x1::x2::tail ->
            let d = dist x1 x2
            let acc' = acc + d
            let xs'' = x2::tail
            lengthRec xs'' acc' first
        | x1'::[] ->
            acc + dist x1' first
        | [] -> acc
    let xs = xArray |> Array.toList
    let firstPoint = xs.Head
    lengthRec xs 0.0 firstPoint