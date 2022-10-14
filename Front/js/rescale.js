let scale = d3.scaleTime()
        .domain([new Date(Date.now() - 60000), Date.now()])  //option + shift+ 5 pour [] sur apple
        .range([0, 500]);

let axis = d3.axisBottom(scale).ticks(d3.timeSecond.every(5));

function updateScaleDomain() {
	let min = new Date(Date.now() - 60000);
	let max = Date.now();
	scale.domain([min, max]);
	update();
}

function update() {
	d3.select('svg g')
		.transition()
		.call(axis);
}

update();
