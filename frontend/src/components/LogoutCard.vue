<template>
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Logout</h5>
            <p> Leave this place and have a rest... </p>
            <a href="#head" type="button" class="btn btn-danger" @click="logout()">Logout</a>
        </div>
    </div>
</template>

<script>
export default {
    methods: {
        async logout() {
            var response = await fetch(`${this.$root.apiUrl}/api/logout`, {
                method: "POST"
            });

            if (response.status == 200) {
                // redirect
                this.$parent.failed = false;
                this.$root.activePage = this.$root.appPage.ROOT;
            } else {
                this.$parent.failed = true;
                this.$parent.message = "Server error. Please try again later.";
            }
        }
    }
}
</script>
