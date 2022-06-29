//timeParse = d3.timeParse("%Y-%m-%dT%H:%M:%S%Z")
const width = 960;
const height = 500;

const margin = { top: 20, right: 20, bottom: 20, left: 50 };


let dataArray = [];


//globals
var dataIndex = 3;
var xBuffer = 50;
var yBuffer = 150;
var lineLength = 400;


//create main svg element
var svgDoc = d3.select("body").append("svg")
    .attr("height", height).attr("width", width)

svgDoc.append("text")
    .attr("x", xBuffer + (lineLength / 2))
    .attr("y", 50)
    .text("dataset");

//create axis line
svgDoc.append("line")
    .attr("x1", xBuffer)
    .attr("y1", yBuffer)
    .attr("x1", xBuffer + lineLength)
    .attr("y2", yBuffer)

//create basic circles
svgDoc.append("g").selectAll("circle")
    .data(dataArray)
    .enter()
    .append("circle")
    .attr("cx", function(d, i) {
        var spacing = lineLength / (data);
        return xBuffer + (i * spacing)
    })
    .attr("cy", yBuffer)
    .attr("r", function(d, i) { return d });

//button to swap over datasets
d3.select("body").append("button")
    .text("change data")
    .on("click", function() {
        pushData();
    }); //end click function

function updateGraphic(data) {
    //select new data
    // if (dataIndex == 3) {
    //     dataIndex = 2;
    // } else {
    //     dataIndex = 3;
    // }

    //rejoin data

    var circle = svgDoc.select("g").selectAll("circle")
        .data(data);

    circle
        .exit()
        .transition(1500)
        .attr("fill", "red")
        .attr("r", 0)
        .remove();
    //remove unneeded circles
    circle.enter()
        .append("circle")
        .attr("r", 0)
        .attr("cx", function(d, i) {
            var spacing = lineLength / (data.length);
            return xBuffer + (i * spacing)
        })
        .attr("cy", yBuffer)
        // 就位
        .transition()
        .duration(1500)
        .attr("r", function(d, i) { return d })
        //放大
    circle.transition()
        .duration(500)
        .attr("cx", function(d, i) {
            var spacing = lineLength / (data.length);
            return xBuffer + (i * spacing)
        })
        .attr("cy", yBuffer)
        .attr("r", function(d, i) { return d });


    //d3.select("text").text(eval(data));
}

function pushData() {
    dataArray.push(5);
    if (dataArray.length > 10) {
        dataArray.slice(0, 1)
    }
    updateGraphic(dataArray);
}

setInterval(pushData(), 1500);