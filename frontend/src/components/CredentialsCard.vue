<template>
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Change Credentials</h5>
            <div class="mb-3">
                <label for="oldPasswordInput" class="form-label">Old Password</label>
                <input type="password" v-model="oldPassword" class="form-control" id="oldPasswordInput"
                    placeholder="Password">
            </div>
            <div class="mb-3">
                <label for="usernameInput" class="form-label">New Username</label>
                <input type="text" v-model="newUsername" class="form-control" id="usernameInput" placeholder="Leave it blank if you don't want to change it">
            </div>
            <div class="mb-3">
                <label for="newPasswordInput" class="form-label">New Password</label>
                <input type="password" v-model="newPassword" class="form-control" id="newPasswordInput"
                    placeholder="Leave it blank if you don't want to change it">
            </div>
            <a href="#head" type="button" class="btn btn-primary" @click="changeCredentials()">Login</a>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            newUsername: '',
            oldPassword: '',
            newPassword: ''
        }
    },
    methods: {
        async changeCredentials() {
            var data = {
                'oldUsername': this.$root.user,
                'newUsername': this.newUsername === '' ? null : this.newUsername,
                'oldPassword': this.oldPassword,
                'newPassword': this.newPassword === '' ? null : this.newPassword
            };
            var response = await fetch(`${this.$root.apiUrl}/api/account`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            if (response.status == 200) {
                // redirect
                this.$parent.failed = false;
                this.$parent.message = "You have successfully changed your credentials.";
            } else if (response.status == 401) {
                this.$parent.failed = true;
                this.$parent.message = "Invalid password.";
            } else {
                this.$parent.failed = true;
                this.$parent.message = "Server error. Please try again later.";
            }
        }
    }
}
</script>
