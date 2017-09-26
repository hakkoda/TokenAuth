class HomeController {
    constructor($resource, $accountService) {
        this.message = 'Hello from the home page!';
        this.value = this.getValue($resource, $accountService);
    }

    getValue(resource, accountService) {
        let valuesResource = resource("/api/values/:id", {}, {
            get: {
                method: 'GET',
                headers: { 'Authorization': 'bearer ' + accountService.getToken() }
            }
        });
        return valuesResource.get({ id: 1 });
    }
}