const epoch_el = document.getElementById("epoch");
const counter_el = document.getElementById("counter");
const last_el = document.getElementById("last_delete");
const up_el = document.getElementById("up");

const iso_to_local = (iso) => {
    let date = new Date(iso)
    return date;
}


var data = {
    epoch: iso_to_local(epoch_el.innerHTML),
    counter: Number.parseInt(counter_el.innerHTML),
    last_delete: iso_to_local(last_el.innerHTML),
    up: up_el.innerHTML,
};

const redraw = () => {
    epoch_el.innerHTML = data.epoch.toLocaleString();
    counter_el.innerHTML = data.counter.toString();
    if (data.last_delete > data.epoch) {
        last_el.innerHTML = data.last_delete.toLocaleString();
    }
    up_el.innerHTML = data.up;
}

const get_data = async () => {
    let req = new Request("/status", {
        method: "GET"
    });
    let res = await fetch(req).catch(() => data.up = "<b class=\"err\">⚠️ The service appears to be down!</b>");
    if (res.status === 200) {
        let res_obj = await res.json();
        if (res_obj.lastDelete != null) {
            data.last_delete = iso_to_local(res_obj.lastDelete);
        }
        data.counter = res_obj.counter;
    }
}

redraw();
setInterval(() => {
    get_data().then(() => redraw())
}, 10000);