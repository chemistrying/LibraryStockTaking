<template>
    <div class="container-fluid text-center col-3">
        <div class="mb-3">
            <label for="usernameInput" class="form-label">Username</label>
            <input type="text" v-model="username" class="form-control" id="usernameInput" placeholder="Username">
        </div>
        <div class="mb-3">
            <label for="passwordInput" class="form-label">Password</label>
            <input type="password" v-model="password" class="form-control" id="passwordInput" placeholder="Password">
        </div>
        <div class="row row-cols-1 g-1">
            <div class="col">
                <button class="btn btn-primary" @click.prevent="loginAccount()">Login</button>
            </div>
            <div class="col">
                <button class="btn btn-primary" @click.prevent="registerAccount()">Register</button>
            </div>
            <div class="col text-danger" v-if="failed">
                <p> {{ failedMessage }} </p>
            </div>
        </div>
    </div>

</template>

<script>
export default {
    data() {
        return {
            failed: false,
            failedMessage: "",
            username: '',
            password: ''
        }
    },
    methods: {
        async loginAccount() {
            var data = {
                'username': this.username, 
                'password': this.password
            };
            var response = await fetch(`${this.$root.apiUrl}/api/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            if (response.status == 200) {
                // redirect
                this.failed = false;
                this.$root.user = this.username;
                this.$root.activePage = this.$root.appPage.ROOT;
            } else if (response.status == 401) {
                this.failed = true;
                this.failedMessage = "Invalid username or password";
            } else {
                this.failed = true;
                this.failedMessage = "Server error. Please try again later.";
            }
        },
        async registerAccount() {
            var data = {
                'username': this.username, 
                'password': this.password
            };
            var response = await fetch(`${this.$root.apiUrl}/api/account`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });
            
            if (response.status == 200) {
                this.$root.user = this.username;
                this.$root.activePage = this.$root.appPage.ROOT;
            } else if (response.status == 400) {
                this.failed = true;
                this.failedMessage = await response.text();
            } else {
                this.failed = true;
                this.failedMessage = "Server error. Please try again later.";
            }
        }
    }
}
</script>