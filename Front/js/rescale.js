let dataArray = [{ value: 0.5, logDate: new Date(Date.now() - 1000) }, { value: 0.6, logDate: new Date(Date.now()) }];
let sensors = d3.select("#sensors_chart");
let xscale = d3.scaleTime().domain([new Date(Date.now() - 60000), Date.now()])//option + shift+ 5 pour [] sur apple
    .range([0, 500]);
let yscale = d3.scaleLinear().domain([-5, 5]).range([200, 10]);
let xAxis = d3.axisTop(xscale).ticks(d3.timeSecond.every(5));
let yAxis = d3.axisLeft(yscale).ticks(2);
const gx = sensors.append("g").attr("transform", `translate(20,  200)`)
    .attr("class", "sensor_xAxis");
const gy = sensors.append("g").attr("transform", "translate(20,0)")
    .attr("class", "sensor_yAxis")

const gDot = sensors.append('g').attr("class", "gDot")
// let dot = sensors.append('g').selectAll("circle")
//     .data(dataArray)  // 
//     .enter()
//     .append("circle")
//     .attr("cx", d => {
//         return xscale(d.logDate)
//     })
//     .attr("cy", d => {
//         return yscale(d.value);
//     })
//     .attr("r", 1.5)


function update(data) {
    d3.select('.sensor_xAxis')
        .transition()
        .call(xAxis);
    d3.select(".sensor_yAxis")
        .transition()
        .call(yAxis);
    let dot = gDot.selectAll("circle").data(data);
    let t = d3.transition().duration(750);

    dot.exit().attr("class", "exit")
        .transition(t)
        .attr("cx", d => xscale(d.logDate))
        .attr("cy", d => yscale(d.value))
        .attr("r", 1.5)
        .style("fill-opacity", 1e-6)
        .remove();
    dot.attr("class", "update")
        .attr("cx", d => xscale(d.logDate))
        .attr("cy", d => yscale(d.value))
        .style("fill-opacity", 1)
        .transition(t);

    dot.enter().append("circle")
        .attr("class", "enter")

        .attr("cx", d => {
            return xscale(d.logDate)
        })
        .attr("cy", d => {
            return yscale(d.value);
        })
        .attr("r", 1.5)
        .transition(t)
        .style("fill", d => {
            let color;
            dataArray.length > 100 ? color = "green" : color = "black";
            return color
        })

}

update(dataArray);

function updateScaleDomain(data) {
    let Xmin = new Date(Date.now() - 60000);
    let Xmax = Date.now();
    let Ymin = d3.max(dataArray, d => d.value) + 20;
    let Ymax = d3.min(dataArray, d => d.value) - 20;
    xscale.domain([Xmin, Xmax]);
    yscale.domain([Ymin, Ymax]);
    update(data);
}

setInterval(function () {
    let data = { value: Math.random() * 100, logDate: new Date(Date.now()) };
    dataArray.push(data);
    updateScaleDomain(dataArray);



}, 5000);//run this thang every 2 seconds